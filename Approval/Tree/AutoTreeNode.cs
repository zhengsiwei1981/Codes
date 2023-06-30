using GJS.Service.Approval.Condation;
using GJS.Service.Approval.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Tree
{
    public class AutoTreeNode : TreeNode
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ICondationBlock> CondationBlocks
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="_context"></param>
        public AutoTreeNode(int nodeId, ApprovalContext _context) : base(nodeId, _context)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        internal override void Initliaze()
        {
            base.Initliaze();
            this.CondationBlocks = new List<ICondationBlock>();
            this.CondationBlocks.AddRange(FieldCondationBlock.LoadBlock(this.Context, this.NodeId));
            this.CondationBlocks.AddRange(AssemblyCondationBlock.LoadBlock(this.Context, this.NodeId));
            this.CondationBlocks.AddRange(SqlCondationBlock.LoadSqlBlock(this.Context, this.NodeId));
        }
    }
}
