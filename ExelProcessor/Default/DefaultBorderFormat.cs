using NPOI.SS.UserModel;
using GJS.Infrastructure.Utility.NOPIFactory.Interface;
using GJS.Infrastructure.Utility.NOPIFactory.Attribute;

namespace GJS.Infrastructure.Utility.NOPIFactory.Default
{
    public class DefaultBorderFormat : IBorderFormat
    {
        public void Format(NOPIContext context, ICellStyle style, CellDescriptionAttribute description)
        {
            style.BorderLeft = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
        }
    }
}
