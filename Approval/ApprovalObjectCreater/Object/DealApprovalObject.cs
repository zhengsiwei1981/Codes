using System.ComponentModel;

namespace GJS.Service.Approval.ApprovalObjectCreater.Object
{
    [Description("Deal")]
    public class DealApprovalObject 
    {
        [Description("合同类型")]
        public string DealTypeName
        {
            get;set;
        }
        [Description("合同序号")]
        public int DealId
        {
            get;set;
        }
    }
}
