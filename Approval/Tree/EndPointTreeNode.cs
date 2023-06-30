using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Tree
{
    public class EndPointTreeNode : TreeNode
    {
        public EndPointTreeNode(int nodeId, ApprovalContext _context) : base(nodeId, _context)
        {
        }
    }
}
