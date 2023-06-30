using GJS.Data.Base.NetRube.Data;
using GJS.Entity;
using GJS.Service.Approval.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.ApprovalArray
{
    public class ApprovalArray
    {
        internal ApprovalContext Context
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public NodeApprovalEmpArrayEntity Array
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public List<ApprovalPerson> Persons
        {
            get; set;
        }
        public TreeNode InFlowNode
        {
            get; set;
        }
        public ApprovalArray(int nodeId, ApprovalContext _context)
        {
            this.Context = _context;
            this.Init(nodeId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeId"></param>
        public void Init(int nodeId)
        {
            this.Array = this.Context.GJSystemDbContext.Get<NodeApprovalEmpArrayEntity>().Where(a => a.NodeId == nodeId).FirstOrDefault();
            if (this.Array != null)
            {
                this.InFlowNode = this.Context.ApprovalTree.Tree.Where(n => n.NodeId == this.Array.InFlowNodeId).FirstOrDefault();
                this.LoadPerson();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadPerson()
        {
            this.Persons = new List<ApprovalPerson>();
            var pList = this.Context.GJSystemDbContext.Get<ApprovalArrayPersonEntity>().Where(p => p.ApprovalArrayId == this.Array.ApprovalArrayId).ToList();
            pList.ForEach(p =>
            {
                this.Persons.Add(new ApprovalPerson(p, this.Context));
            });
        }
    }
}
