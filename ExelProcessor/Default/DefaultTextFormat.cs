using GJS.Infrastructure.Utility.NOPIFactory.Attribute;
using GJS.Infrastructure.Utility.NOPIFactory.Interface;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace GJS.Infrastructure.Utility.NOPIFactory.Default
{
    public class DefaultTextFormat : ITextFormat
    {
        public void Format(NOPIContext context,ICellStyle style, CellDescriptionAttribute description)
        {
            if (description.CellType != CellType.Auto)
            {
                switch (description.CellType)
                {
                    case CellType.DateTime:
                        style.DataFormat = context.WorkBook.CreateDataFormat().GetFormat("yyyy年m月d日");
                        break;
                    case CellType.Float:
                        style.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                        break;
                    case CellType.Percentage:
                        style.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                        break;
                    case CellType.ScientificNotation:
                        style.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00E+00");
                        break;
                    case CellType.ChineseNumeral:
                        style.DataFormat = context.WorkBook.CreateDataFormat().GetFormat("[DbNum2][$-804]General");
                        break;
                    case CellType.Currency:
                        if (description.CurrencySign == null)
                        {
                            style.DataFormat = context.WorkBook.CreateDataFormat().GetFormat("#,##0");
                        }
                        else
                        {
                            style.DataFormat = context.WorkBook.CreateDataFormat().GetFormat(description.CurrencySign);
                        }
                        break;
                }
            }
        }
    }
}
