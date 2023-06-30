using GJS.Infrastructure.Utility.NOPIFactory;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Infrastructure.Utility.NOPI.Interface
{
    public interface ICustomReplace
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        void Replace(object obj, string name, ParagraphTextSetter setter,XWPFParagraph paragraph);
    }
}
