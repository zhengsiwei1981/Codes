using GJS.Data.Base.NetRube.Data;
using GJS.Entity;
using GJS.Infrastructure.Enum;
using GJS.Infrastructure.Utility;
using GJS.Service.Approval.ApprovalObjectCreater.Object;
using GJS.Service.Approval.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.ApprovalObjectCreater
{
    public class DealInsertApprovalObjectCreater : ICreateApprovalObject
    {
        public object Create(ApprovalContext context, int id)
        {
            //模拟数据
            var dealEntity = context.GJSystemDbContext.Get<DealEntity>().Where(d => d.DealId == id).FirstOrDefault();
            return new DealApprovalObject()
            {
                DealTypeName = ((DealTypeEnum)dealEntity.DealType).ToDescription(),
                DealId = dealEntity.DealId
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Create(ApprovalContext context, int id, Type type)
        {
            return null;
        }

        public string GenerateUrl(ApprovalContext context, int id)
        {
            return "";
        }
    }
}
