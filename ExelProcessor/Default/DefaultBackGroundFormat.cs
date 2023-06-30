using GJS.Infrastructure.Utility.NOPIFactory.Attribute;
using GJS.Infrastructure.Utility.NOPIFactory.Interface;
using NPOI.SS.UserModel;

namespace GJS.Infrastructure.Utility.NOPIFactory.Default
{
    public class DefaultBackGroundFormat : IBackGroundFormat
    {
        public void Format(NOPIContext context, ICellStyle style, CellDescriptionAttribute description)
        {
            //style.FillPattern = FillPattern.SolidForeground;
            //style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
            //style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
        }
    }
}
