using NPOI.SS.UserModel;
using GJS.Infrastructure.Utility.NOPIFactory.Interface;
using GJS.Infrastructure.Utility.NOPIFactory.Attribute;

namespace GJS.Infrastructure.Utility.NOPIFactory.Default
{
    public class DefaulAlignmentFormat : IAlignmentFormat
    {
        public void Format(NOPIContext context, ICellStyle style,CellDescriptionAttribute description)
        {
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
        }
    }
}
