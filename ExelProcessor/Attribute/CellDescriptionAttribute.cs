using GJS.Infrastructure.Utility.NOPIFactory.Default;
using GJS.Infrastructure.Utility.NOPIFactory.Interface;
using NPOI.SS.UserModel;
using System;
using System.Reflection;

namespace GJS.Infrastructure.Utility.NOPIFactory.Attribute
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CellDescriptionAttribute : System.Attribute
    {
        /// <summary>
        /// 顺序
        /// </summary>
        public int Order
        {
            get; set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 文本旋转
        /// </summary>
        public int? Indention
        {
            get; set;
        }
        /// <summary>
        /// 文本缩进
        /// </summary>
        public int? Rotation
        {
            get; set;
        }
        /// <summary>
        /// 自动换行
        /// </summary>
        public bool IsWrap
        {
            get; set;
        }
        /// <summary>
        /// 列类型(未设置自定义的文本格式接口时，此枚举将产生作用)
        /// </summary>
        public NOPIFactory.CellType CellType
        {
            get; set;
        }
        /// <summary>
        /// 文本格式
        /// </summary>
        public Type TextFormat
        {
            get; set;
        }
        /// <summary>
        /// 字体格式
        /// </summary>
        public Type FontFormat
        {
            get; set;
        }
        /// <summary>
        /// 边框格式
        /// </summary>
        public Type BorderFormat
        {
            get; set;
        }
        /// <summary>
        /// 背景格式
        /// </summary>
        public Type BackGroudFormat
        {
            get; set;
        }
        /// <summary>
        /// 对齐方式
        /// </summary>
        public Type Alignment
        {
            get; set;
        }
        /// <summary>
        /// 货币符号
        /// </summary>
        public string CurrencySign
        {
            get; set;
        }
        /// <summary>
        /// 属性
        /// </summary>
        internal PropertyInfo PropertyInfo
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        private ITextFormat TextFormater
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        private IFontFormat FontFormater
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        private IBorderFormat BorderFormater
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        private IBackGroundFormat BackGroundFormater
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        private IAlignmentFormat AlignmentFormater
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal ICellStyle Style
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public CellDescriptionAttribute(int order, string name)
        {
            this.Order = order;
            this.Name = name;

            this.TextFormater = this.TextFormat == null ? new DefaultTextFormat() : (ITextFormat)Activator.CreateInstance(this.TextFormat);
            this.FontFormater = this.FontFormat == null ? new DefaultFontFormat() : (IFontFormat)Activator.CreateInstance(this.FontFormat);
            this.BorderFormater = this.BorderFormat == null ? new DefaultBorderFormat() : (IBorderFormat)Activator.CreateInstance(this.BorderFormat);
            this.BackGroundFormater = this.BackGroudFormat == null ? new DefaultBackGroundFormat() : (IBackGroundFormat)Activator.CreateInstance(this.BackGroudFormat);
            this.AlignmentFormater = this.Alignment == null ? new DefaulAlignmentFormat() : (IAlignmentFormat)Activator.CreateInstance(this.Alignment);
        }

        public CellDescriptionAttribute(int order, string name, string propertyName) : this(order, name)
        {
            this.PropertyName = propertyName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="context"></param>
        public virtual void OnCellRender(ICell cell, NOPIContext context)
        {
            if (this.Style == null)
            {
                this.Style = context.WorkBook.CreateCellStyle();
                if (this.Indention.HasValue)
                {
                    this.Style.Indention = (short)this.Indention.GetValueOrDefault();
                }
                if (this.Rotation.HasValue)
                {
                    this.Style.Rotation = (short)this.Rotation.GetValueOrDefault();
                }
                this.Style.WrapText = this.IsWrap;

                this.TextFormater.Format(context, this.Style, this);
                this.FontFormater.Format(context, this.Style, this);
                this.BorderFormater.Format(context, this.Style, this);
                this.BackGroundFormater.Format(context, this.Style, this);
                this.AlignmentFormater.Format(context, this.Style, this);
            }
            cell.CellStyle = this.Style;
        }
    }
}