using GJS.Data.Base.NetRube.Data;
using GJS.Entity;
using GJS.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Tree
{
    public class ApprovalTree
    {
        internal ApprovalContext Context
        {
            get; set;
        }
        public string TreeNodeDescription
        {
            get; set;
        }
        public List<TreeNode> Tree
        {
            get; set;
        }
        public string NodeSubordinate
        {
            get; set;
        }
        public ApprovalTree(ApprovalContext _context)
        {
            this.Context = _context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_nodeSubordinate"></param>
        /// <param name="_context"></param>
        public ApprovalTree(string _nodeSubordinate, ApprovalContext _context) : this(_context)
        {
            this.Context.ApprovalTree = this;
            this.LoadTreeWithNoLevel(_nodeSubordinate);
            this.Tree = this.Tree.OrderBy(n => n.Node.NodeType).ToList();
            this.Tree.ForEach(n => { n.Initliaze(); });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_nodeSubordinate"></param>
        private void LoadTreeWithNoLevel(string _nodeSubordinate)
        {
            this.NodeSubordinate = _nodeSubordinate;
            this.Tree = new List<TreeNode>();

            var nodeList = this.Context.GJSystemDbContext.Get<ApprovalNodeEntity>().Where(n => n.NodeSubordinate == this.NodeSubordinate && n.FlowId == this.Context.ApprovalObject.Entity.FlowId).Select(n => new { n.NodeId, n.NodeType }).ToList<dynamic>();
            nodeList.ForEach(n =>
            {
                this.Tree.Add(TreeNode.Create(this.Context, n.NodeId, (NodeTypeEnum)n.NodeType));
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void AttachApprovalObject()
        {
            var node = this.Tree.Where(n => n.Node.NodeType == (int)NodeTypeEnum.TriggerPoint).FirstOrDefault();
            node.Action.Receive(this.Context, node);
        }
    }
}
