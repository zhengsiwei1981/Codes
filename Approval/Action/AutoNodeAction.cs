using GJS.Service.Approval.Interface;
using GJS.Service.Approval.Tree;
using GJS.Service.Approval.ObjectReleation;
using GJS.Service.Approval.NodeAttribute;
using GJS.Infrastructure.Enum;

namespace GJS.Service.Approval.Action
{
    [NodeActionType(NodeTypeEnum.Automatic)]
    public class AutoNodeAction : INodeAction
    {
        public void Execute(ApprovalContext context, TreeNode node)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="node"></param>
        public void Receive(ApprovalContext context, TreeNode node)
        {
            ObjectNodeReleation nodeReleation = new ObjectNodeReleation(context);
            nodeReleation.AttachFlow(node);
            var autoTreeNode = (AutoTreeNode)node;
            foreach (var block in autoTreeNode.CondationBlocks)
            {
                var result = block.Determine(context.ApprovalObject.Object);
                if (result)
                {
                    nodeReleation.ResetFlowState(node, Infrastructure.Enum.ConfirmStateEnum.Pass);
                    block.InFlowNode.Action.Receive(context, block.InFlowNode);
                    return;
                }
            }
            nodeReleation.ResetFlowState(node, Infrastructure.Enum.ConfirmStateEnum.Reject);
            node.Reject.Action.Receive(context, node.Reject);
        }
    }
}
