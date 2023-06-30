using GJS.Entity;
using GJS.Service.Approval.Interface;
using System.Collections.Generic;
using System.Linq;
using GJS.Service.Approval.Tree;
using GJS.Data.Base.NetRube.Data;

namespace GJS.Service.Approval.Condation
{
    public class SqlCondationBlock : ICondationBlock
    {
        /// <summary>
        /// 
        /// </summary>
        public ApprovalContext Context { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public BlockSqlCondationEntity Block { get; set; }
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
            ///需要增加Id
            var result = this.Context.GJSystemDbContext.Execute(string.Format(this.Block.Sql, this.Context.ApprovalObject.ObjectId));
            return result == 1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_context"></param>
        /// <param name="block"></param>
        public SqlCondationBlock(ApprovalContext _context, BlockSqlCondationEntity block)
        {
            this.Context = _context;
            this.Block = block;
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
        public static List<SqlCondationBlock> LoadSqlBlock(ApprovalContext context, int nodeId)
        {
            var dbBlockList = context.GJSystemDbContext.Get<BlockSqlCondationEntity>().Where(s => s.NodeId == nodeId).ToList();
            var blockList = new List<SqlCondationBlock>();
            dbBlockList.ForEach(b =>
            {
                blockList.Add(new SqlCondationBlock(context, b));
            });
            return blockList;
        }
    }
}
