using GJS.Service.Approval.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GJS.Service.Approval.Tree;
using GJS.Entity;
using GJS.Data.Base.NetRube.Data;
using GJS.Service.Approval.NodeAttribute;
using GJS.Infrastructure.Enum;

namespace GJS.Service.Approval.Action
{
    [NodeActionType(NodeTypeEnum.TriggerPoint)]
    public class TriggerPointNodeAction : INodeAction
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
            var children = context.ApprovalTree.Tree.Where(n => n.Node.ParentNodeId == node.NodeId).FirstOrDefault();
            if (children != null)
            {
                children.Action.Receive(context, children);
            }
        }
    }
}
