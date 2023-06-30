using GJS.Data.Base.NetRube.Data;
using GJS.Entity;
using GJS.Infrastructure.Enum;
using GJS.Service.Approval.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.ObjectReleation
{
    public class ObjectNodeReleation
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
        private List<ApprovalObjectNodeRelationEntity> ReleationList
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ObjectNodeReleation(ApprovalContext context)
        {
            this.Context = context;
            this.Init();
        }
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            this.ReleationList = this.Context.GJSystemDbContext.Get<ApprovalObjectNodeRelationEntity>().Where(r => r.ApprovalObjectId == this.Context.ApprovalObject.Entity.ApprovalObjectId).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        public void AttachFlow(TreeNode node)
        {
            int arrayId = 0;
            if (node.Node.NodeType == (int)NodeTypeEnum.Artificial)
            {
                arrayId = ((ArtificialTreeNode)node).ApprovalArray.Array.ApprovalArrayId;
            }
            this.Context.GJSystemDbContext.Add(new ApprovalObjectNodeRelationEntity()
            {
                ApprovalObjectId = this.Context.ApprovalObject.Entity.ApprovalObjectId,
                ConfirmState = (int)ConfirmStateEnum.Undo,
                NodeId = node.NodeId,
                ApprovalArrayId = arrayId
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public void ResetFlowState(TreeNode node, ConfirmStateEnum confirmStatue)
        {
            var releation = this.Context.GJSystemDbContext.Get<ApprovalObjectNodeRelationEntity>().Where(r => r.ApprovalObjectId == this.Context.ApprovalObject.Entity.ApprovalObjectId && r.NodeId == node.NodeId).FirstOrDefault();
            releation.ConfirmState = (int)confirmStatue;
            this.Context.GJSystemDbContext.Set(releation);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal TreeNode CurrentEnableNode()
        {
            var releation = this.Context.GJSystemDbContext.Get<ApprovalObjectNodeRelationEntity>().Where(n => n.ApprovalObjectId == this.Context.ApprovalObject.Entity.ApprovalObjectId
               && (n.ConfirmState == (int)ConfirmStateEnum.Undo)).FirstOrDefault();
            if (releation == null)
            {
                return this.Context.ApprovalTree.Tree.Where(t => t.Type == NodeTypeEnum.TriggerPoint).FirstOrDefault();
            }

            return this.Context.ApprovalTree.Tree.Where(n => n.NodeId == releation.NodeId).FirstOrDefault();
        }
        /// <summary>
        /// 撤销
        /// </summary>
        internal void Cancel()
        {
            this.Context.GJSystemDbContext.Del<ApprovalObjectNodeRelationEntity>(n => n.ApprovalObjectId == this.Context.ApprovalObject.Entity.ApprovalObjectId);
        }
    }
}
