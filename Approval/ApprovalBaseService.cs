using GJS.Infrastructure.CommonModel;
using GJS.Infrastructure.CommonModel.Exception;
using System;

namespace GJS.Service.Approval
{
    public abstract class ApprovalBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="func"></param>
        protected virtual R Execute<R>(Func<ApprovalContext, R> func)
        {
            R result = default(R);
            ApprovalContext context = new ApprovalContext() {  };
            try
            {
                context.GJSystemDbContext.BeginTransaction();
                result = func(context);
                context.GJSystemDbContext.CompleteTransaction();
            }
            catch
            {
                context.GJSystemDbContext.AbortTransaction();
                throw;
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="func"></param>
        protected virtual void Execute(Action<ApprovalContext> func)
        {
            this.Execute<bool>(c =>
            {
                func(c);
                return true;
            });
        }
    }
}
