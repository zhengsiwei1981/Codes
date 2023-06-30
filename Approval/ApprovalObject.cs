using GJS.Data.Base.NetRube.Data;
using GJS.Entity;
using GJS.Infrastructure.CommonModel;
using GJS.Infrastructure.CommonModel.Exception;
using GJS.Infrastructure.Enum;
using GJS.Infrastructure.Utility;
using GJS.Service.Approval.Comment;
using GJS.Service.Approval.Flow;
using GJS.Service.Approval.Interface;
using GJS.Service.Approval.ObjectReleation;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GJS.Service.Approval
{
    public class ApprovalObject
    {
        internal ApprovalObjectEntity Entity
        {
            get; set;
        }
        public ApprovalContext Context
        {
            get; set;
        }
        public ObjectComments Comments
        {
            get; set;
        }
        public ApprovalFlowEntity Flow
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Subordinate
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public object Object
        {
            get; set;
        }
        public int ObjectId
        {
            get; set;
        }
        public ICreateApprovalObject ObjectCreater
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int ApprovalObjectId
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ApprovalObject(int id, string subordinate, ICreateApprovalObject objectCreater, ApprovalContext _context, bool isCreate, int approvalObjectId = 0)
        {
            this.Context = _context;
            this.Context.ApprovalObject = this;
            this.ObjectId = id;
            this.Subordinate = subordinate;
            this.ApprovalObjectId = approvalObjectId;
            if (isCreate)
            {
                if (IsExist())
                {
                    throw new BusinessException(BusinessStatusCode.RepeatFlow);
                }
                this.ObjectCreater = objectCreater;
            }
            this.Init();
        }
        /// <summary>
        /// 
        /// </summary>
        public void InitComments()
        {
            this.Comments = new ObjectComments(this.Context);
        }
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            if (IsExist())
            {
                ApprovalFlow flow = new ApprovalFlow(this.Context);
                if (this.ApprovalObjectId == 0)
                {
                    this.Entity = this.Context.GJSystemDbContext.Get<ApprovalObjectEntity>().Where(o => o.ApprovalBusId == this.ObjectId && o.NodeSubordinate == this.Subordinate).FirstOrDefault();
                }
                else
                {
                    this.Entity = this.Context.GJSystemDbContext.Get<ApprovalObjectEntity>().Where(o => o.ApprovalObjectId == this.ApprovalObjectId && o.NodeSubordinate == this.Subordinate).FirstOrDefault();
                }

                this.Flow = flow.Create(this.Entity.FlowId);
                this.Context.TriggerAction = (TriggerActionEnum)this.Flow.TriggerAction;
                var obj = Assembly.Load(this.Context.AssemblyPath).CreateInstance(this.Entity.ApprovalObjectType);
                this.Object = XmlHelper.DeSerialize(this.Entity.ApprovalBusObject, obj.GetType());
            }
            else
            {
                if (this.ObjectCreater != null)
                {
                    this.Object = this.ObjectCreater.Create(this.Context, this.ObjectId);
                    this.CreateApprovalObjectToDB();
                }
                //else
                //{
                //    throw new BusinessException(BusinessStatusCode.CanNotApproval);
                //}
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void CreateApprovalObjectToDB()
        {
            ApprovalFlow flow = new ApprovalFlow(this.Context);
            flow.MatchApprovalObject(this.Object);
            this.Flow = flow.EnableFlow;
            this.Entity = new ApprovalObjectEntity();
            if (flow.EnableFlow != null)
            {
                this.Entity.FlowId = flow.EnableFlow.FlowId;
            }
            this.Entity.ApprovalBusId = this.ObjectId;
            this.Entity.ApprovalBusObject = XmlHelper.Serialize(this.Object);
            this.Entity.NodeSubordinate = this.Subordinate;
            this.Entity.ApprovalUrl = this.ObjectCreater.GenerateUrl(this.Context, this.ObjectId);
            this.Entity.ApprovalAppUrl = this.ObjectCreater.GenerateAppUrl(this.Context, this.ObjectId);
            this.Entity.ApprovalObjectType = this.Object.GetType().ToString();

            if (flow.EnableFlow != null)
            {
                this.Context.GJSystemDbContext.Add<ApprovalObjectEntity>(this.Entity);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsExist()
        {
            var approvalObjList = this.Context.GJSystemDbContext.Get<ApprovalObjectEntity>().Where(o => o.NodeSubordinate == this.Subordinate && o.ApprovalBusId == this.ObjectId).ToList();
            if (approvalObjList == null || approvalObjList.Count < 1)
            {
                return false;
            }
            if (approvalObjList.Count == 1)
            {
                var isExist = this.Context.GJSystemDbContext.Get<ApprovalObjectNodeRelationEntity>().Where(n => n.ApprovalObjectId == approvalObjList[0].ApprovalObjectId
                && (n.ConfirmState == (int)ConfirmStateEnum.Undo)).Exist();
                return isExist;
            }
            else
            {
                Dictionary<int, bool> objectMapExist = new Dictionary<int, bool>();
                approvalObjList.ForEach(a =>
                {
                    var isExist = this.Context.GJSystemDbContext.Get<ApprovalObjectNodeRelationEntity>().Where(n => n.ApprovalObjectId == a.ApprovalObjectId
                && (n.ConfirmState == (int)ConfirmStateEnum.Undo)).Exist();
                    objectMapExist.Add(a.ApprovalObjectId, isExist);
                });
                var existCount = objectMapExist.Where(kv => kv.Value).Count();
                if (existCount > 1)
                {
                    throw new BusinessException(BusinessStatusCode.RepeatFlow);
                }
                else
                {
                    if (existCount == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            new ObjectNodeReleation(this.Context).Cancel();
            new ApprovalPersonNodeReleation(this.Context, null).Cancel();
            this.Context.GJSystemDbContext.Del(this.Entity);
        }
    }
}
