using GJS.Service.Approval.Interface;
using System.Collections.Generic;
using GJS.Service.Approval.Tree;
using GJS.Service.Approval.ObjectReleation;
using GJS.Infrastructure.Enum;
using GJS.Service.Approval.NodeAttribute;
using GJS.Service.Message;
using GJS.Entity;
using GJS.Infrastructure.Utility;
using GJS.Infrastructure.Utility.AppPush;
using GJS.Data.Base;
using GJS.Data.Base.NetRube.Data;
using GJS.Data.Base.NetRube.Data.Ext;

namespace GJS.Service.Approval.Action
{
    [NodeActionType(NodeTypeEnum.PositionPoint)]
    public class PositionNodeAction : INodeAction
    {
        public void Execute(ApprovalContext context, TreeNode node)
        {
            var positionNode = (PositionTreeNode)node;
            if (positionNode.Position.Entity.ApprovalMethod == (int)ApprovalMethodEnum.Single)
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
                    positionNode.Position.InFlowNode.Action.Receive(context, positionNode.Position.InFlowNode);
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
                        positionNode.Position.InFlowNode.Action.Receive(context, positionNode.Position.InFlowNode);
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

            personReleation.AttachPersonReleationFromPosistion();
            nodeReleation.AttachFlow(node);

            //发邮件通知等
            MessageService service = new MessageService();
            var pNode = ((PositionTreeNode)node);
            if (pNode != null)
            {
                List<int> empIds = new List<int>();
                if (pNode.Position.Entity.IsDepartmentLeader)
                {
                    empIds.Add(pNode.Position.Department.DepartmentLeaderId.GetValueOrDefault());
                }
                else
                {
                    empIds = pNode.Position.EmpIds;
                }

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
                service.Add(messageEntity, empIds);
            }
        }
    }
}
