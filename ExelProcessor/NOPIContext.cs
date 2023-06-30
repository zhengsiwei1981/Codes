using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace GJS.Infrastructure.Utility.NOPIFactory
{
    public class NOPIContext
    {
        /// <summary>
        /// 
        /// </summary>
        public IWorkbook WorkBook
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal DocumentDescription Descripter
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public TitleStyleRender TitleSytle
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public object Data
        {
            get;set;
        }
        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get;set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal ActiveRow TitleRow
        {
            get;set;
        }
        internal IFormulaEvaluator Evaluator
        {
            get;set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string,ICellStyle> CustomStyles
        {
            get;set;
        }
        public NOPIContext()
        {
            this.CustomStyles = new Dictionary<string, ICellStyle>();
        }
    }
}
