using GJS.Service.Approval.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.PropertyValueProvider
{
    public class DefaultValueProvider : IRefResourceProvider
    {
        public List<object> GetValue()
        {
            return new List<object>();
        }
    }
}
