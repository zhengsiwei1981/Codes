using NPOI.SS.UserModel;

namespace GJS.Infrastructure.Utility.NOPIFactory.Attribute
{
    public class SheetDescriptionAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? DefaultWidth
        {
            get;set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? DefaultHeight
        {
            get;set;
        }
        public virtual void OnSheetRender(ISheet sheet, NOPIContext Context)
        {
            if (this.DefaultHeight.HasValue)
            {
                sheet.DefaultRowHeight = (short)(this.DefaultHeight.GetValueOrDefault() * 20);
            }
            if (this.DefaultWidth.HasValue)
            {
                sheet.DefaultColumnWidth = this.DefaultWidth.GetValueOrDefault() * 256;
            }         
        }
    }
}
