using GJS.Infrastructure.Utility.NOPIFactory.Attribute;
using NPOI.SS.UserModel;

namespace GJS.Infrastructure.Utility.NOPIFactory.Interface
{
    public interface IAlignmentFormat
    {
        void Format(NOPIContext context,ICellStyle cellStyle, CellDescriptionAttribute description);
    }
}
