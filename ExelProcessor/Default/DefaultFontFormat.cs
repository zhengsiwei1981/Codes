using GJS.Infrastructure.Utility.NOPIFactory.Attribute;
using GJS.Infrastructure.Utility.NOPIFactory.Interface;
using NPOI.SS.UserModel;

namespace GJS.Infrastructure.Utility.NOPIFactory.Default
{
    public class DefaultFontFormat : IFontFormat
    {
        public void Format(NOPIContext context,ICellStyle style, CellDescriptionAttribute description)
        {
            var font = context.WorkBook.CreateFont();
            font.FontName = "微软雅黑";
            font.FontHeightInPoints = 12;
            font.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
            style.SetFont(font);
        }
    }
}
