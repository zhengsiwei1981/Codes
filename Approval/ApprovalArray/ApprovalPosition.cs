using GJS.Data.Base.NetRube.Data;
using GJS.Entity;
using GJS.Infrastructure.CommonModel.Exception;
using GJS.Infrastructure.Enum;
using GJS.Service.Approval.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.ApprovalArray
{
    public class ApprovalPosition
    {
        /// <summary>
        /// 
        /// </summary>
        public ApprovalContext Context
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ApprovalPositionEntity Entity
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public DepartmentEntity Department
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public TreeNode InFlowNode
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public List<int> EmpIds
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public ApprovalPosition(ApprovalContext context, int nodeId)
        {
            this.Context = context;
            this.Init(nodeId);
        }
        /// <summary>
        /// 
        /// </summary>
        private void Init(int nodeId)
        {
            this.Entity = this.Context.GJSystemDbContext.Get<ApprovalPositionEntity>().Where(p => p.NodeId == nodeId).FirstOrDefault();
            if (this.Entity != null)
            {
                this.InFlowNode = this.Context.ApprovalTree.Tree.Where(n => n.NodeId == this.Entity.InFlowNodeId).FirstOrDefault();
                if (this.Entity.IsDepartmentLeader == false)
                {
                    this.Department = this.Context.GJSystemDbContext.Get<DepartmentEntity>().Where(d => d.DepartmentId == this.Entity.PositionId).FirstOrDefault();
                    if (this.Department != null)
                    {
                        this.EmpIds = this.Context.GJSystemDbContext.Get<EmployeeEntity>().Where(e => e.DeptId == this.Department.DepartmentId).Select(e => e.EmployeeId).ToList<int>();
                    }
                }
                else
                {
                    var creater = this.Context.GJSystemDbContext.Get<EmployeeEntity>().Where(e => e.EmployeeId == this.Context.ApprovalObject.Entity.Creator).FirstOrDefault();
                    if (creater != null)
                    {
                        this.Department = this.Context.GJSystemDbContext.Get<DepartmentEntity>().Where(d => d.DepartmentId == creater.DeptId).FirstOrDefault();
                        if (this.Department.DataStatus == (int)DataStatusEnum.Disable)
                        {
                            throw new BusinessException("部门已被禁用！");
                        }
                        if (this.Department.DepartmentLeaderId == null)
                        {
                            throw new BusinessException("未指定部门负责人！");
                        }
                    }
                }
            }
            else
            {
                throw new BusinessException("未获取指定的审批节点！");
            }
        }
    }
}
