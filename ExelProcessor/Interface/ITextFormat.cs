using GJS.Infrastructure.Utility.NOPIFactory.Attribute;
using NPOI.SS.UserModel;

namespace GJS.Infrastructure.Utility.NOPIFactory.Interface
{
    public interface ITextFormat
    {
        void Format(NOPIContext context, ICellStyle cellStyle, CellDescriptionAttribute description);
    }
}
