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

namespace GJS.Service.Approval.ObjectReleation
{
    public class ApprovalPersonNodeReleation
    {
        public ApprovalContext Context
        {
            get; set;
        }
        public TreeNode Node
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public ApprovalPersonNodeReleation(ApprovalContext context, TreeNode node)
        {
            this.Context = context;
            this.Node = node;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool AllConfirm()
        {
            return this.Context.GJSystemDbContext.Get<ApprovalPersonNodeRelationEntity>().Where(a => a.ApprovalObjectId == this.Context.ApprovalObject.Entity.ApprovalObjectId
            && a.NodeId == Node.NodeId).ToList().All(r => r.ConfirmState == (int)ConfirmStateEnum.Pass);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool AnyConfirm()
        {
            return this.Context.GJSystemDbContext.Get<ApprovalPersonNodeRelationEntity>().Where(a => a.ApprovalObjectId == this.Context.ApprovalObject.Entity.ApprovalObjectId
            && a.NodeId == Node.NodeId).ToList().Any(r => r.ConfirmState == (int)ConfirmStateEnum.Pass);
        }
        /// <summary>
        /// 
        /// </summary>
        public void SetConfirmState(int empId, ConfirmStateEnum confirmState, bool updateOtherUserState)
        {
            var releation = this.Context.GJSystemDbContext.Get<ApprovalPersonNodeRelationEntity>().Where(a => a.ApprovalObjectId == this.Context.ApprovalObject.Entity.ApprovalObjectId
           && a.NodeId == Node.NodeId && a.EmpId == empId).FirstOrDefault();
            releation.ConfirmState = (int)confirmState;
            releation.CompleteMethod = (int)CompleteMethodEnum.Artificial;

            this.Context.GJSystemDbContext.Set(releation);

            if (updateOtherUserState)
            {
                this.GetInnerList().Where(u => u.EmpId != empId).ToList().ForEach(r =>
                {
                    if (r.ConfirmState == (int)ConfirmStateEnum.Undo)
                    {
                        r.ConfirmState = (int)confirmState;
                        r.CompleteMethod = (int)CompleteMethodEnum.Automatic;
                        this.Context.GJSystemDbContext.Set(r);
                    }
                });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void SetReadState(int empId,bool read)
        {
            var releation = this.Context.GJSystemDbContext.Get<ApprovalPersonNodeRelationEntity>().Where(a => a.ApprovalObjectId == this.Context.ApprovalObject.Entity.ApprovalObjectId
           && a.NodeId == Node.NodeId && a.EmpId == empId).FirstOrDefault();
            if (releation != null)
            {
                releation.IsRead = read;
                this.Context.GJSystemDbContext.Set(releation);
            }
            else
            {
                throw new BusinessException("未找到当前节点下的此用户信息！");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="confirmState"></param>
        public void SetConfirmState(ConfirmStateEnum confirmState)
        {
            this.GetInnerList().ForEach(r =>
            {
                r.ConfirmState = (int)confirmState;
                r.CompleteMethod = (int)CompleteMethodEnum.Automatic;
                this.Context.GJSystemDbContext.Set(r);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<ApprovalPersonNodeRelationEntity> GetInnerList()
        {
            return this.Context.GJSystemDbContext.Get<ApprovalPersonNodeRelationEntity>().Where(a => a.ApprovalObjectId == this.Context.ApprovalObject.Entity.ApprovalObjectId
           && a.NodeId == Node.NodeId).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        public void AttachPersonReleationFromNode()
        {
            ((ArtificialTreeNode)this.Node).ApprovalArray.Persons.ForEach(p =>
            {
                this.Context.GJSystemDbContext.Add(new ApprovalPersonNodeRelationEntity()
                {
                    ApprovalObjectId = this.Context.ApprovalObject.Entity.ApprovalObjectId,
                    CompleteMethod = (int)CompleteMethodEnum.Undo,
                    ConfirmState = (int)ConfirmStateEnum.Undo,
                    NodeId = this.Node.NodeId,
                    EmpId = p.Person.EmpId,
                    ApprovalPersonId = p.Person.PersonArrayId
                });
            });
        }
        /// <summary>
        /// 
        /// </summary>
        public void AttachPersonReleationFromPosistion()
        {
            var positionNode = this.Node as PositionTreeNode;
            if (positionNode.Position.Entity.IsDepartmentLeader)
            {
                if (positionNode.Position.Department != null)
                {
                    this.Context.GJSystemDbContext.Add(new ApprovalPersonNodeRelationEntity()
                    {
                        ApprovalObjectId = this.Context.ApprovalObject.Entity.ApprovalObjectId,
                        CompleteMethod = (int)CompleteMethodEnum.Undo,
                        ConfirmState = (int)ConfirmStateEnum.Undo,
                        NodeId = this.Node.NodeId,
                        EmpId = positionNode.Position.Department.DepartmentLeaderId.GetValueOrDefault(),
                        ApprovalPersonId = ((PositionTreeNode)this.Node).Position.Department.DepartmentId
                    });
                }
                else
                {
                    throw new BusinessException("未找到相关部门负责人信息！");
                }
            }
            else
            {
                positionNode.Position.EmpIds.ForEach(empId =>
                {
                    if (empId == 0)
                    {
                        throw new Exception("未获取到员工信息！");
                    }
                    this.Context.GJSystemDbContext.Add(new ApprovalPersonNodeRelationEntity()
                    {
                        ApprovalObjectId = this.Context.ApprovalObject.Entity.ApprovalObjectId,
                        CompleteMethod = (int)CompleteMethodEnum.Undo,
                        ConfirmState = (int)ConfirmStateEnum.Undo,
                        NodeId = this.Node.NodeId,
                        EmpId = empId,
                        ApprovalPersonId = positionNode.Position.Department.DepartmentId
                    });
                });
            }
        }
        /// <summary>
        /// 撤销
        /// </summary>
        internal void Cancel()
        {
            this.Context.GJSystemDbContext.Del<ApprovalPersonNodeRelationEntity>(n => n.ApprovalObjectId == this.Context.ApprovalObject.Entity.ApprovalObjectId);
        }
    }
}
