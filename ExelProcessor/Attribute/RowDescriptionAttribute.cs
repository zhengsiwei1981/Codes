using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Infrastructure.Utility.NOPIFactory.Attribute
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RowDescriptionAttribute : System.Attribute
    {
        public virtual void OnRowRender(IRow row,NOPIContext context)
        {
        
        }
    }
}
