using GJS.Data.Base;
using GJS.Entity;
using GJS.Infrastructure.Enum;
using GJS.Infrastructure.Enum.EnumAttribute;
using GJS.Infrastructure.Utility;
using GJS.Service.Approval.Tree;
using PetaPoco;

namespace GJS.Service.Approval
{
    public class ApprovalContext
    {
        private DbContextFactory factory = new DbContextFactory();
        /// <summary>
        /// 数据库上下文
        /// </summary>
        public Database GJSystemDbContext
        {
            get
            {
                return DbContextFactory.Default;
            }
        }
        private NodeSubordinateEnum _subordinate;
        /// <summary>
        /// 
        /// </summary>
        public NodeSubordinateEnum Subordinate
        {
            get
            {
                return _subordinate;
            }
            set
            {
                _subordinate = value;
                var attr = _subordinate.GetAttribute<AssemblySetAttribute>();
                this.TheSubordinate = attr.TheSubordinate;
                this.AssemblyPath = attr.AssemblyPath;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public TriggerActionEnum TriggerAction
        {
            get;set;
        }
        public string TheSubordinate
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string AssemblyPath
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ApprovalTree ApprovalTree
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ApprovalObject ApprovalObject
        {
            get; set;
        }
        public EmployeeEntity User
        {
            get; set;
        }
        public bool UserApprovalResult
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSpecial
        {
            get;set;
        }
    }
}
