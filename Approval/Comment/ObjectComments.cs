using GJS.Data.Base.NetRube.Data;
using GJS.Entity;
using GJS.Service.Approval.ObjectReleation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Comment
{
    public class ObjectComments
    {
        public ApprovalContext Context
        {
            get; set;
        }
        public List<ApprovalCommentEntity> Comments
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public ObjectComments(ApprovalContext context)
        {
            this.Context = context;
            this.Init();
        }
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            this.Comments = new List<ApprovalCommentEntity>();
            ObjectNodeReleation releation = new ObjectNodeReleation(this.Context);
            var currentNode = releation.CurrentEnableNode();
            if (currentNode != null)
            {
                this.Comments = this.Context.GJSystemDbContext.Get<ApprovalCommentEntity>().Where(a => a.ApprovalObjectId == this.Context.ApprovalObject.Entity.ApprovalObjectId && a.NodeId == currentNode.NodeId).ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comment"></param>
        public void Attach(string comment)
        {
            ObjectNodeReleation releation = new ObjectNodeReleation(this.Context);
            ApprovalCommentEntity entity = new ApprovalCommentEntity()
            {
                ApprovalObjectId = this.Context.ApprovalObject.Entity.ApprovalObjectId,
                Comments = comment,
                EmpId = this.Context.User.EmployeeId,
                NodeId = releation.CurrentEnableNode().NodeId,
                ApprovalArrayId = 0
            };
            this.Context.GJSystemDbContext.Add<ApprovalCommentEntity>(entity);
            this.Comments.Add(entity);
        }
    }
}
