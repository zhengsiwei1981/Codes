using GJS.Entity;
using GJS.Service.Approval.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GJS.Service.Approval.Tree;
using GJS.Data.Base.NetRube.Data;
using GJS.Data.Base;
using System.Linq.Expressions;
using GJS.Data.Base.DynamicExpression.Interface;
using System.Reflection;
using GJS.Data.Base.DynamicExpression;

namespace GJS.Service.Approval.Condation
{
    public class FieldCondationBlock : ICondationBlock
    {
        public NodeCondationBlockEntity NodeCondationBlock { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ApprovalContext Context { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<BlockFieldCondationEntity> CondationList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TreeNode InFlowNode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="approvalObj"></param>
        /// <returns></returns>
        public bool Determine(object approvalObj)
        {
            BuilderAssembly builderAssembly = new BuilderAssembly();
            var genericTypeMapping = typeof(Lambad<>).MakeGenericType(approvalObj.GetType());
            var lambad = Activator.CreateInstance(genericTypeMapping);

            var parameterExpression = Expression.Parameter(approvalObj.GetType(), "obj");
            this.CondationList.ForEach(condation =>
            {
                var property = approvalObj.GetType().GetProperty(condation.FieldName);
                var memberExpr = Expression.Lambda(Expression.Property(parameterExpression, property), parameterExpression);
                var gMethod = lambad.GetType()
                .GetMethod("WithAnd").MakeGenericMethod(property.PropertyType)
                .Invoke(lambad, new object[] { memberExpr, GetValue(condation, approvalObj), builderAssembly.CreateOperatorInstance(condation.CompareOperater) });
            });

            var expr = lambad.GetType().GetMethod("Create").Invoke(lambad, null);
            var func = expr.GetType().GetMethod("Compile", new Type[] { }).Invoke(expr, null);
            var result = func.GetType().GetMethod("Invoke").Invoke(func, new object[] { approvalObj });

            return (bool)result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private object GetValue(BlockFieldCondationEntity condationEntity, object approvalObj)
        {
            if (condationEntity.CompareFieldName != null && condationEntity.CompareFieldName.Length > 0)
            {
                return approvalObj.GetType().GetProperty(condationEntity.CompareFieldName).GetValue(approvalObj);
            }
            var fieldType = condationEntity.FieldType;
            if (fieldType.Contains("System.Collections.Generic.List`1"))
            {
                var p = approvalObj.GetType().GetProperty(condationEntity.FieldName);
                fieldType = p.PropertyType.GenericTypeArguments[0].FullName;
            }
            switch (fieldType)
            {
                case "System.Int32":
                    return Convert.ChangeType(condationEntity.CompareValue, typeof(int));
                case "System.String":
                    return condationEntity.CompareValue;
                case "System.Decimal":
                    return Convert.ChangeType(condationEntity.CompareValue, typeof(decimal));
                case "System.DateTime":
                    return DateTime.Parse(condationEntity.CompareValue);
                default:
                    return condationEntity.CompareValue;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_context"></param>
        /// <param name="block"></param>
        public FieldCondationBlock(ApprovalContext _context, NodeCondationBlockEntity block)
        {
            this.Context = _context;
            this.NodeCondationBlock = block;
            this.Init();
        }
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            this.CondationList = this.Context.GJSystemDbContext.Get<BlockFieldCondationEntity>().Where(b => b.BlockId == this.NodeCondationBlock.BlockId).ToList();
            this.InFlowNode = this.Context.ApprovalTree.Tree.Where(n => n.NodeId == this.NodeCondationBlock.InFlowNodeId).FirstOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static List<FieldCondationBlock> LoadBlock(ApprovalContext context, int nodeId)
        {
            var dbBlockList = context.GJSystemDbContext.Get<NodeCondationBlockEntity>().Where(b => b.NodeId == nodeId).ToList();
            var blockList = new List<FieldCondationBlock>();
            dbBlockList.ForEach(b =>
            {
                blockList.Add(new FieldCondationBlock(context, b));
            });
            return blockList;
        }
    }
}
