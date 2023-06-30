using GJS.Entity;
using GJS.Service.Approval.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GJS.Service.Approval.Tree;
using GJS.Data.Base.NetRube.Data;
using System.Reflection;

namespace GJS.Service.Approval.Condation
{
    public class AssemblyCondationBlock : ICondationBlock
    {
        /// <summary>
        /// 
        /// </summary>
        public ApprovalContext Context { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public BlockAssemblyCondationEntity Block { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TreeNode InFlowNode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="approvalObj"></param>
        /// <returns></returns>
        public bool Determine(object approvalObj)
        {
            var condationExecuter = (IAssemblyCondation)Assembly.Load(this.Context.AssemblyPath).CreateInstance(this.Block.AssemblyPath);
            return condationExecuter.Execute(this.Context);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_context"></param>
        /// <param name="block"></param>
        public AssemblyCondationBlock(ApprovalContext _context, BlockAssemblyCondationEntity block)
        {
            this.Block = block;
            this.Context = _context;
            this.Init();
        }
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            this.InFlowNode = this.Context.ApprovalTree.Tree.Where(n => n.NodeId == this.Block.InFlowNodeId).FirstOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static List<AssemblyCondationBlock> LoadBlock(ApprovalContext context, int nodeId)
        {
            var dbBlockList = context.GJSystemDbContext.Get<BlockAssemblyCondationEntity>().Where(a => a.NodeId == nodeId).ToList();
            var blockList = new List<AssemblyCondationBlock>();
            dbBlockList.ForEach(b =>
            {
                blockList.Add(new AssemblyCondationBlock(context, b));
            });
            return blockList;
        }
    }
}
