using GJS.Entity;
using GJS.Service.Approval.ApprovalArray;
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
    public class PositionTreeNode : TreeNode
    {
        public ApprovalPosition Position
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public PositionTreeNode(int nodeId, ApprovalContext _context) : base(nodeId, _context)
        {
        }

        internal override void Initliaze()
        {
            base.Initliaze();
            this.Position = new ApprovalPosition(this.Context, this.NodeId);
        }
    }
}
