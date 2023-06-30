using GJS.Service.Approval.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GJS.Service.Approval.Tree;
using GJS.Service.Approval.NodeAttribute;
using GJS.Infrastructure.Enum;

namespace GJS.Service.Approval.Action
{
    [NodeActionType(NodeTypeEnum.EndPoint)]
    public class EndPointNodeAction : INodeAction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="node"></param>
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
            //通知
            ObjectReleation.ObjectNodeReleation releation = new ObjectReleation.ObjectNodeReleation(context);
            releation.AttachFlow(node);
            releation.ResetFlowState(node, Infrastructure.Enum.ConfirmStateEnum.End);
        }
    }
}
