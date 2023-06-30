using GJS.Data.Base.NetRube.Data;
using GJS.Entity;
using GJS.Infrastructure.CommonModel;
using GJS.Infrastructure.CommonModel.Exception;
using GJS.Infrastructure.Enum;
using GJS.Service.Approval.Flow;
using GJS.Service.Approval.Interface;
using GJS.Service.Approval.ObjectReleation;
using GJS.Service.Approval.Tree;

namespace GJS.Service.Approval
{
    public class ApprovalService : ApprovalBaseService
    {
        /// <summary>
        /// 创建审批流
        /// </summary>
        /// <param name="id">审批业务对象序号</param>
        /// <param name="subordinate">审批归属（所属业务）</param>
        /// <param name="creater">创建审批对象的接口</param>
        public ApprovalResult CreateApproval(int id, TriggerActionEnum action, NodeSubordinateEnum subordinate, ICreateApprovalObject creater, bool isSpecial = false)
        {
            return this.Execute<ApprovalResult>(context =>
            {
                context.Subordinate = subordinate;
                context.TriggerAction = action;
                context.IsSpecial = isSpecial;

                ApprovalObject approvalObj = new ApprovalObject(id, context.TheSubordinate, creater, context, true);
                if (approvalObj.Flow == null)
                {
                    return new ApprovalResult() { MatchFlow = false };
                }
                ApprovalTree approvalTree = new ApprovalTree(context.TheSubordinate, context);
                approvalObj.InitComments();
                approvalTree.AttachApprovalObject();
                return new ApprovalResult() { MatchFlow = true };
            });
        }
        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="id">审批业务对象序号</param>
        /// <param name="subordinate">审批归属（所属业务）</param>
        /// <param name="approvalReulst">审批结果</param>
        /// <param name="comments">备注</param>
        public void Approval(int id, int userId, NodeSubordinateEnum subordinate, bool approvalReulst, string comments = null, int approvalObjectId = 0)
        {
            this.Execute(context =>
            {
                context.Subordinate = subordinate;
                context.UserApprovalResult = approvalReulst;
                context.User = context.GJSystemDbContext.Get<EmployeeEntity>().Where(e => e.EmployeeId == userId).FirstOrDefault();

                ApprovalObject approvalObj = new ApprovalObject(id, context.TheSubordinate, null, context, false, approvalObjectId);
                ApprovalTree approvalTree = new ApprovalTree(context.TheSubordinate, context);
                approvalObj.InitComments();
                ObjectNodeReleation releation = new ObjectNodeReleation(context);

                if (comments != null)
                {
                    approvalObj.Comments.Attach(comments);
                }
                var node = releation.CurrentEnableNode();
                node.Action.Execute(context, node);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        public void ReadProcess(int id, int approvalObjectId, int userId, NodeSubordinateEnum subordinate)
        {
            this.Execute(context =>
            {
                context.Subordinate = subordinate;
                ApprovalObject approvalObj = new ApprovalObject(id, context.TheSubordinate, null, context, false, approvalObjectId);
                ApprovalTree approvalTree = new ApprovalTree(context.TheSubordinate, context);
                if (approvalObj.Entity == null)
                {
                    throw new BusinessException("当前审批流已结束，不能设置已读状态！");
                }
                ObjectNodeReleation releation = new ObjectNodeReleation(context);
                var node = releation.CurrentEnableNode();

                ApprovalPersonNodeReleation personNodeReleation = new ApprovalPersonNodeReleation(context, node);
                personNodeReleation.SetReadState(userId, true);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="approvalObjectid"></param>
        public void Cancel(NodeSubordinateEnum subordinate, int approvalObjectid, int approvalBusId, ICancellation cancellation)
        {
            this.Execute(context =>
            {
                if (cancellation == null)
                {
                    throw new BusinessException(BusinessStatusCode.CancelInvalid);
                }
                context.Subordinate = subordinate;
                ApprovalObject approvalObj = new ApprovalObject(approvalBusId, context.TheSubordinate, null, context, false, approvalObjectid);
                if (approvalObj.IsExist())
                {
                    approvalObj.Cancel();
                    cancellation.Cancel(approvalObj);
                }
                else
                {
                    throw new BusinessException(BusinessStatusCode.CanNotCancelApproval);
                }
            });
        }
    }
}
