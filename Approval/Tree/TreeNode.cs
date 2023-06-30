using GJS.Data.Base.NetRube.Data;
using GJS.Entity;
using GJS.Infrastructure.CommonModel.Exception;
using GJS.Infrastructure.Enum;
using GJS.Service.Approval.Interface;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace GJS.Service.Approval.Tree
{
    public abstract class TreeNode
    {
        internal ApprovalNodeEntity Node
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal ApprovalContext Context
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public NodeTypeEnum Type
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public INodeAction Action
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public TreeNode Parent
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public TreeNode Reject
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int NodeId
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_context"></param>
        public TreeNode(ApprovalContext _context)
        {
            this.Context = _context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="_context"></param>
        public TreeNode(int nodeId, ApprovalContext _context) : this(_context)
        {
            this.InitWithNoLevel(nodeId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeId"></param>
        internal void InitWithNoLevel(int nodeId)
        {
            this.NodeId = nodeId;
            this.Node = this.Context.GJSystemDbContext.Get<ApprovalNodeEntity>().Where(a => a.NodeId == nodeId).FirstOrDefault();
            if (this.Node == null)
            {
                throw new NullObjectException("未发现节点信息！");
            }
            this.Type = (NodeTypeEnum)this.Node.NodeType;

            try
            {
                var obj = Assembly.Load(this.Node.OperateAssemblyName).CreateInstance(this.Node.ReceiptOperateAssembly);
                this.Action = (INodeAction)obj;
            }
            catch (Exception ex)
            {
                throw new ParameterException(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        virtual internal void Initliaze()
        {
            this.Parent = this.Context.ApprovalTree.Tree.FirstOrDefault(n => n.NodeId == this.Node.ParentNodeId);
            this.Reject = this.Context.ApprovalTree.Tree.FirstOrDefault(n => n.NodeId == this.Node.RejectNodeId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_context"></param>
        /// <param name="nodeId"></param>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        internal static TreeNode Create(ApprovalContext _context, int nodeId, NodeTypeEnum nodeType)
        {
            switch (nodeType)
            {
                case NodeTypeEnum.Artificial:
                    return new ArtificialTreeNode(nodeId, _context);
                case NodeTypeEnum.Automatic:
                    return new AutoTreeNode(nodeId, _context);
                case NodeTypeEnum.TriggerPoint:
                    return new TriggerPointTreeNode(nodeId, _context);
                case NodeTypeEnum.EndPoint:
                    return new EndPointTreeNode(nodeId, _context);
                case NodeTypeEnum.FailPoint:
                    return new FailTreeNode(nodeId, _context);
                case NodeTypeEnum.PositionPoint:
                    return new PositionTreeNode(nodeId, _context);
                default:
                    return null;
            }
        }
    }
}
