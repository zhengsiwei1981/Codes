using GJS.Data.Base;
using GJS.Data.Base.DynamicExpression;
using GJS.Data.Base.NetRube.Data;
using GJS.Entity;
using GJS.Infrastructure.CommonModel;
using GJS.Infrastructure.CommonModel.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Flow
{
    public class ApprovalFlow
    {
        /// <summary>
        /// 
        /// </summary>
        internal List<ApprovalFlowTriggerEntity> CondationList
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal ApprovalFlowEntity EnableFlow
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
        public ApprovalFlow(ApprovalContext context)
        {
            this.Context = context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ApprovalFlowEntity Create(int id)
        {
            this.EnableFlow = this.Context.GJSystemDbContext.Get<ApprovalFlowEntity>().Where(f => f.FlowId == id).FirstOrDefault();
            return this.EnableFlow;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="approvalObject"></param>
        public void MatchApprovalObject(object approvalObject)
        {
            var flowList = this.Context.GJSystemDbContext.Get<ApprovalFlowEntity>().Where(f => f.TriggerAction == (int)this.Context.TriggerAction && f.Subordinate == (int)this.Context.Subordinate && f.Enable == true && f.IsSpecial == this.Context.IsSpecial).ToList();
            foreach (var flow in flowList)
            {
                this.CondationList = this.Context.GJSystemDbContext.Get<ApprovalFlowTriggerEntity>().Where(ft => ft.FlowId == flow.FlowId).ToList();
                if (this.CondationList != null && this.CondationList.Count > 0)
                {
                    if (this.Match(approvalObject))
                    {
                        this.EnableFlow = flow;
                        return;
                    }
                }
                else
                {
                    this.EnableFlow = flow;
                    return;
                }
            }
            //if (this.EnableFlow == null)
            //{
            //    throw new BusinessException(BusinessStatusCode.NotMatchApprovalFlow);
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="approvalObject"></param>
        private bool Match(object approvalObject)
        {
            BuilderAssembly builderAssembly = new BuilderAssembly();
            var genericTypeMapping = typeof(Lambad<>).MakeGenericType(approvalObject.GetType());
            var lambad = Activator.CreateInstance(genericTypeMapping);

            var parameterExpression = Expression.Parameter(approvalObject.GetType(), "obj");
            this.CondationList.ForEach(condation =>
            {
                var property = approvalObject.GetType().GetProperty(condation.FieldName);
                var memberExpr = Expression.Lambda(Expression.Property(parameterExpression, property), parameterExpression);
                var gMethod = lambad.GetType()
                .GetMethod("WithAnd").MakeGenericMethod(property.PropertyType)
                .Invoke(lambad, new object[] { memberExpr, GetValue(condation, approvalObject), builderAssembly.CreateOperatorInstance(condation.CompareOperater) });
            });

            var expr = lambad.GetType().GetMethod("Create").Invoke(lambad, null);
            var func = expr.GetType().GetMethod("Compile", new Type[] { }).Invoke(expr, null);
            var result = func.GetType().GetMethod("Invoke").Invoke(func, new object[] { approvalObject });

            return (bool)result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private object GetValue(ApprovalFlowTriggerEntity condationEntity, object approvalObj)
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
    }
}
