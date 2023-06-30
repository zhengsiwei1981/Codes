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
    public class DealUpdateApprovalObjectCreater : ICreateApprovalObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public object Create(ApprovalContext context, int id)
        {
            var dealEntity = context.GJSystemDbContext.Get<DealModifyEntity>().Where(d => d.DealModifyId == id).FirstOrDefault();
            var obj = XmlHelper.DeSerialize<object>(dealEntity.ModifyData);
            return new DealApprovalObject()
            {
                DealTypeName = ((DealTypeEnum)obj.GetType().GetProperty("DealType").GetValue(obj)).ToDescription(),
                DealId = (int)obj.GetType().GetProperty("DealId").GetValue(obj)
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
            var dealEntity = context.GJSystemDbContext.Get<DealModifyEntity>().Where(d => d.DealModifyId == id).FirstOrDefault();
            var obj = XmlHelper.DeSerialize(dealEntity.ModifyData, type);
            return new DealApprovalObject()
            {
                DealTypeName = ((DealTypeEnum)obj.GetType().GetProperty("DealType").GetValue(obj)).ToDescription(),
                DealId = (int)obj.GetType().GetProperty("DealId").GetValue(obj)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GenerateUrl(ApprovalContext context, int id)
        {
            return "";
        }
    }
}
