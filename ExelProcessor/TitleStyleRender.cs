using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Infrastructure.Utility.NOPIFactory
{
    /// <summary>
    /// 
    /// </summary>
    public class TitleStyleRender
    {
        public virtual void Format(NOPIContext context, ICell cell)
        {
            var style = context.WorkBook.CreateCellStyle();
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;

            style.Alignment = HorizontalAlignment.Center;
            var font = context.WorkBook.CreateFont();
            font.FontName = "微软雅黑";
            font.FontHeightInPoints = 12;
            font.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
            style.SetFont(font);

            style.FillPattern = FillPattern.SolidForeground;           
            //style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;         
            style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;

            cell.CellStyle = style;
        }
    }
}
