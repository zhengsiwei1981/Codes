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
    [NodeActionType(NodeTypeEnum.FailPoint)]
    public class FailNodeAction : INodeAction
    {
        public void Execute(ApprovalContext context, TreeNode node)
        {
            
        }

        public void Receive(ApprovalContext context, TreeNode node)
        {
            ObjectReleation.ObjectNodeReleation releation = new ObjectReleation.ObjectNodeReleation(context);
            releation.AttachFlow(node);
            releation.ResetFlowState(node, Infrastructure.Enum.ConfirmStateEnum.Reject);
            //通知
        }
    }
}
