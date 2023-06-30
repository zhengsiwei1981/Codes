using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GJS.Infrastructure.Utility.NOPIFactory
{
    /// <summary>
    /// 渲染模板
    /// </summary>
    public class ExcelTemplate
    {
        #region 把结算数据填充单excel中
        public static void RenderSettlementExportFile(string templatePath, object dataSource, string saveAsPath)
        {
            FileStream fs = new FileStream(templatePath, FileMode.OpenOrCreate);
            XSSFWorkbook workBook = new XSSFWorkbook(fs);
            ISheet sheet = workBook.GetSheetAt(0);

            //列表数据出现在哪行
            int dataRowIndex = 0;
            //列表数据有多少行
            int dataRowNum = 0;
            //数据行映射实体名称
            string dataRowMapEntityName = "";
            if (dataSource != null)
            {
                for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;

                    if (row.Cells != null && row.Cells.Count > 0)
                    {
                        //判断普通字段标记
                        row.Cells.ForEach(c =>
                        {
                            if (c.CellType == NPOI.SS.UserModel.CellType.String)
                            {
                                var val = c.StringCellValue;
                                var start = val.IndexOf('[');
                                var end = val.IndexOf(']');
                                if (start != -1 && end != -1)
                                {
                                    string proName = val.Substring(start, end - start + 1);
                                    string valBeforeStartIndex = start > 0 ? val.Substring(0, start) : string.Empty;
                                    string valAfterStartIndex = end < val.Length - 1 ? val.Substring(end + 1) : string.Empty;
                                    var mapPro = dataSource.GetType().GetProperty(proName.Replace("[", "").Replace("]", ""));
                                    if (mapPro != null)
                                    {
                                        var mapProValue = mapPro.GetValue(dataSource);
                                        if (mapProValue != null && mapProValue.GetType() == typeof(DateTime))
                                        {
                                            c.SetCellValue((DateTime)Convert.ChangeType(mapProValue, typeof(DateTime)));
                                        }
                                        else if (mapProValue != null && mapProValue.GetType() == typeof(double))
                                        {
                                            var attrs = mapPro.GetCustomAttributes(typeof(ThreePointAttribute), false);
                                            if (attrs != null && attrs.Length > 0)
                                            {
                                                c.SetCellValue(((double)mapProValue).ToString("0.000"));
                                            }
                                            else
                                            {
                                                c.SetCellValue(((double)mapProValue).ToString("0.00"));
                                            }
                                        }
                                        else if (mapProValue != null)
                                        {
                                            c.SetCellValue(valBeforeStartIndex + mapProValue.ToString() + valAfterStartIndex);
                                        }
                                        else if (mapProValue == null)
                                        {
                                            c.SetCellValue("");
                                        }
                                    }
                                }
                            }
                        });

                        //判断是否数据行
                        bool isDataRow = false;
                        foreach (var cell in row.Cells)
                        {
                            if (cell.CellType == NPOI.SS.UserModel.CellType.Formula)
                            {
                                continue;
                            }
                            var val = cell.StringCellValue;
                            if (!string.IsNullOrEmpty(val) && val[0] == '$' && val[val.Length - 1] == '$')
                            {
                                isDataRow = true;
                                dataRowIndex = rowIndex;
                                dataRowMapEntityName = val.Replace("$", "").Split(new char[] { '.' })[0];
                                break;
                            }

                        }

                        //如果是映射数据行
                        if (isDataRow)
                        {
                            //获取数据行对象
                            var dataRowProperty = dataSource.GetType().GetProperty(dataRowMapEntityName);
                            //判断是否是真实数据行
                            if (dataRowProperty.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                //获取集合
                                var list = dataRowProperty.GetValue(dataSource);
                                if (list != null && list is IList)
                                {
                                    //获取数据行数
                                    dataRowNum = (list as IList).Count;
                                    if (dataRowNum == 0)
                                    {
                                        //如果为1行数据，则将当前文档内的标记行直接替换为实际内容，数据行标记为 $XXX$
                                        foreach (var cell in row.Cells)
                                        {
                                            if (cell.CellType == NPOI.SS.UserModel.CellType.Formula)
                                            {
                                                continue;
                                            }
                                            var val = cell.StringCellValue;
                                            if (string.IsNullOrEmpty(val)) continue;

                                            if (val.IndexOf('$') != -1)
                                                cell.SetCellValue("");
                                        }
                                    }
                                }
                                else if (list == null)
                                {
                                    //如果为1行数据，则将当前文档内的标记行直接替换为实际内容，数据行标记为 $XXX$
                                    foreach (var cell in row.Cells)
                                    {
                                        if (cell.CellType == NPOI.SS.UserModel.CellType.Formula)
                                        {
                                            continue;
                                        }
                                        var val = cell.StringCellValue;
                                        if (string.IsNullOrEmpty(val)) continue;

                                        if (val.IndexOf('$') != -1)
                                            cell.SetCellValue("");
                                    }
                                }
                            }
                        }
                    }
                }

                //渲染列表数据
                if (dataRowNum > 0)
                {
                    //先插入行
                    for (int i = 0; i < dataRowNum - 1; i++)
                    {
                        sheet.ShiftRows(dataRowIndex + i + 1, sheet.LastRowNum, 1, true, false);
                        ExcelOperate.CopyRow(workBook, sheet, dataRowIndex, dataRowIndex + i + 1);
                    }

                    //再渲染数据
                    var dataRowProperty = dataSource.GetType().GetProperty(dataRowMapEntityName);
                    for (int j = dataRowIndex, i = 0; j < dataRowIndex + dataRowNum; j++, i++)
                    {
                        IRow row = sheet.GetRow(j);
                        //判断是否是真实数据行
                        //if (dataRowProperty.PropertyType == typeof(List<>))
                        //获取集合
                        var list = dataRowProperty.GetValue(dataSource);
                        if (list != null && list is IList)
                        {
                            //如果为1行数据，则将当前文档内的标记行直接替换为实际内容，数据行标记为 $XXX$
                            foreach (var cell in row.Cells)
                            {
                                if (cell.CellType == NPOI.SS.UserModel.CellType.Formula)
                                    continue;
                                var val = cell.StringCellValue;
                                if (string.IsNullOrEmpty(val)) continue;

                                var dataItemName = val.Replace("$", "").Split(new char[] { '.' })[1];
                                var proItem = (list as IList)[0].GetType().GetProperty(dataItemName);
                                if (proItem != null)
                                {
                                    var itemValue = proItem.GetValue((list as IList)[i]);
                                    if (itemValue != null && itemValue.GetType() == typeof(double))
                                    {
                                        var attrs = proItem.GetCustomAttributes(typeof(ThreePointAttribute), false);
                                        if (attrs != null && attrs.Length > 0)
                                        {
                                            cell.SetCellValue(((double)itemValue).ToString("0.000"));
                                        }
                                        else
                                        {
                                            cell.SetCellValue(((double)itemValue).ToString("0.00"));
                                        }
                                    }
                                    else
                                    {
                                        cell.SetCellValue(itemValue == null ? null : itemValue.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }

            fs.Close();
            FileManager manager = new FileManager(saveAsPath);
            using (FileStream fileStream = new FileStream(saveAsPath, FileMode.Create, FileAccess.Write))
            {
                workBook.Write(fileStream);
                workBook.Close();
            }
        }
        #endregion
    }
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ThreePointAttribute : System.Attribute
    {

    }
}
