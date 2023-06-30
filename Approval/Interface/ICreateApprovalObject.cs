using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Interface
{
    public interface ICreateApprovalObject
    {
        object Create(ApprovalContext context,int id);
        string GenerateUrl(ApprovalContext context, int id);
        string GenerateAppUrl(ApprovalContext context, int id);
    }
}
