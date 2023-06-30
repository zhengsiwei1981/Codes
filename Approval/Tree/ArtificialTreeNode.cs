using GJS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Tree
{
    /// <summary>
    /// 
    /// </summary>
    public class ArtificialTreeNode : TreeNode
    {
        public ApprovalArray.ApprovalArray ApprovalArray
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ArtificialTreeNode(int nodeId, ApprovalContext _context) : base(nodeId, _context)
        {
        }

        internal override void Initliaze()
        {
            base.Initliaze();
            this.ApprovalArray = new Approval.ApprovalArray.ApprovalArray(this.NodeId, this.Context);
        }
    }
}
