using GJS.Service.Approval.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Interface
{
    public interface INodeAction
    {
        void Execute(ApprovalContext context,TreeNode node);
        void Receive(ApprovalContext context,TreeNode node);
    }
}
