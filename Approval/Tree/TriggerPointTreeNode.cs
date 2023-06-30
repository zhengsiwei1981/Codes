using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Tree
{
    public class TriggerPointTreeNode : TreeNode
    {
        public TriggerPointTreeNode(int nodeId, ApprovalContext _context) : base(nodeId, _context)
        {
        }
    }
}
