using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Infrastructure.Utility.NOPIFactory
{
    public enum CellType
    {
        /// <summary>
        /// 自动按基础值类型转换
        /// </summary>
        Auto,
        /// <summary>
        /// 日期
        /// </summary>
        DateTime,
        /// <summary>
        /// 浮点数(精度到小数点后2位)
        /// </summary>
        Float,
        /// <summary>
        /// 货币
        /// </summary>
        Currency,
        /// <summary>
        /// 百分比
        /// </summary>
        Percentage,
        /// <summary>
        /// 数字转中文大写
        /// </summary>
        ChineseNumeral,
        /// <summary>
        /// 科学计数法
        /// </summary>
        ScientificNotation
    }
}
