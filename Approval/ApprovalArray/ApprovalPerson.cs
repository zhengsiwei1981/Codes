using GJS.Data.Base.NetRube.Data;
using GJS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.ApprovalArray
{
    public class ApprovalPerson
    {
        internal ApprovalContext Context
        {
            get;set;
        }
        public ApprovalArrayPersonEntity Person
        {
            get;set;
        }
        public EmployeeEntity Employee
        {
            get;set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="_context"></param>
        public ApprovalPerson(ApprovalArrayPersonEntity person,ApprovalContext _context)
        {
            this.Context = _context;
            this.Person = person;
            this.Init();
        }
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            this.Employee = this.Context.GJSystemDbContext.Get<EmployeeEntity>().Where(e => e.EmployeeId == Person.EmpId).FirstOrDefault();
        }
    }
}
