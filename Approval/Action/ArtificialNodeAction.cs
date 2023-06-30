using GJS.Service.Approval.Interface;
using System.Linq;
using GJS.Data.Base;
using GJS.Data.Base.NetRube.Data;
using GJS.Service.Approval.Tree;
using GJS.Service.Approval.ObjectReleation;
using GJS.Infrastructure.Enum;
using GJS.Service.Approval.NodeAttribute;
using GJS.Service.Message;
using GJS.Infrastructure.Utility;
using GJS.Entity;

namespace GJS.Service.Approval.Action
{
    [NodeActionType(NodeTypeEnum.Artificial)]
    public class ArtificialNodeAction : INodeAction
    {
        public void Execute(ApprovalContext context, TreeNode node)
        {
            var artificialNode = (ArtificialTreeNode)node;
            if (artificialNode.ApprovalArray.Array.ApprovalMethod == (int)ApprovalMethodEnum.Single)
            {
                if (!context.UserApprovalResult)
                {
                    this.Reject(context, node);
                }
                else
                {
                    ApprovalPersonNodeReleation personReleation = new ApprovalPersonNodeReleation(context, node);
                    ObjectNodeReleation nodeReleation = new ObjectNodeReleation(context);

                    personReleation.SetConfirmState(context.User.EmployeeId, ConfirmStateEnum.Pass, true);
                    nodeReleation.ResetFlowState(node, ConfirmStateEnum.Pass);
                    artificialNode.ApprovalArray.InFlowNode.Action.Receive(context, artificialNode.ApprovalArray.InFlowNode);
                }
            }
            //All
            else
            {
                if (!context.UserApprovalResult)
                {
                    this.Reject(context, node);
                }
                else
                {
                    ApprovalPersonNodeReleation personReleation = new ApprovalPersonNodeReleation(context, node);
                    ObjectNodeReleation nodeReleation = new ObjectNodeReleation(context);

                    personReleation.SetConfirmState(context.User.EmployeeId, ConfirmStateEnum.Pass, false);
                    var isPass = personReleation.AllConfirm();
                    if (isPass)
                    {
                        nodeReleation.ResetFlowState(node, ConfirmStateEnum.Pass);
                        artificialNode.ApprovalArray.InFlowNode.Action.Receive(context, artificialNode.ApprovalArray.InFlowNode);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void Reject(ApprovalContext context, TreeNode node)
        {
            ApprovalPersonNodeReleation personReleation = new ApprovalPersonNodeReleation(context, node);
            ObjectNodeReleation nodeReleation = new ObjectNodeReleation(context);

            personReleation.SetConfirmState(context.User.EmployeeId, ConfirmStateEnum.Reject, true);
            nodeReleation.ResetFlowState(node, ConfirmStateEnum.Reject);
            node.Reject.Action.Receive(context, node.Reject);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="node"></param>
        public void Receive(ApprovalContext context, TreeNode node)
        {
            ObjectNodeReleation nodeReleation = new ObjectNodeReleation(context);
            ApprovalPersonNodeReleation personReleation = new ApprovalPersonNodeReleation(context, node);

            personReleation.AttachPersonReleationFromNode();
            nodeReleation.AttachFlow(node);
            //发邮件通知等
            MessageService service = new MessageService();
            var empids = ((ArtificialTreeNode)node).ApprovalArray.Persons.Select(p => p.Employee.EmployeeId).ToList<int>();

            var messageEntity = new MessageEntity()
            {
                SmsBody = string.Format("您有{0}的审批消息", context.Subordinate.GetDescription()),
                SmsHead = "审批消息",
                ViewUrl = context.ApprovalObject.Entity.ApprovalUrl + "&approvalObjectId=" + context.ApprovalObject.Entity.ApprovalObjectId + "&subordinate=" + (int)context.Subordinate + "&busId=" + (int)context.ApprovalObject.Entity.ApprovalBusId,
                SmsSendEmpId = context.ApprovalObject.Entity.Creator,
                SmsSendEmpName = DbContextFactory.Default.Get<EmployeeEntity>().Where(p => p.Id == context.ApprovalObject.Entity.Creator).FirstOrDefault()?.FullName,
                SmsTypeId = (int)SmsTypeEnum.Approval,
                SourceId = context.ApprovalObject.Entity.ApprovalObjectId
            };
            service.Add(messageEntity, empids);
        }
    }
}
