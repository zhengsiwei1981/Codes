using GJS.Entity.Base;
using GJS.Service.Approval.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Interface
{
    public interface ICondationBlock
    {
        ApprovalContext Context
        {
            get;set;
        }
        TreeNode InFlowNode
        {
            get; set;
        }
        bool Determine(object approvalObj);
    }
}
