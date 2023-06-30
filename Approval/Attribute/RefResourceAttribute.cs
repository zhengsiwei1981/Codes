using GJS.Infrastructure.Utility;
using GJS.Service.Approval.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Attributes
{
    public class RefResourceAttribute : Attribute
    {
        public IRefResourceProvider ValueProvider;
        public RefResourceAttribute(Type valueProvider)
        {
            if (typeof(IRefResourceProvider).IsAssignableFrom(valueProvider))
            {
                this.ValueProvider = (IRefResourceProvider)Activator.CreateInstance(valueProvider);
            }
            else
            {
                throw new InstanceCreateException();
            }
        }
    }
}
