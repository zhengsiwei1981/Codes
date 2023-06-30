using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using NPOI.HSSF.UserModel;
using System.Reflection;
using GJS.Infrastructure.Utility.NOPIFactory.Attribute;
using System.ComponentModel;
using System.Collections;
using NPOI.SS.Util;

namespace GJS.Infrastructure.Utility.NOPIFactory
{
    public class ExcelFactory
    {
        /// <summary>
        /// 
        /// </summary>
        internal IWorkbook Workbook
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public TitleStyleRender TitleRender
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal NOPIContext Context
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ActiveSheet Sheet
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public IFormulaEvaluator Evaluator
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal FileManager FileManager
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public ExcelFactory(string filePath)
        {
            this.TitleRender = new TitleStyleRender();
            this.FileManager = new FileManager(filePath);
            this.Workbook = this.CreateWorkBook(this.FileManager);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public ExcelFactory(string filePath, TitleStyleRender titleRender)
        {
            this.TitleRender = titleRender;
            this.FileManager = new FileManager(filePath);
            this.Workbook = this.CreateWorkBook(this.FileManager);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private IWorkbook CreateWorkBook(FileManager file)
        {
            IWorkbook wb = null;
            if (file.Extension == ".xls")
            {
                if (file.HasFile)
                {
                    wb = new HSSFWorkbook(new FileStream(file.FilePath, FileMode.Open));
                }
                else
                {
                    wb = new HSSFWorkbook();
                }
                this.Evaluator = new HSSFFormulaEvaluator(wb);
            }
            else if (file.Extension == ".xlsx")
            {
                if (file.HasFile)
                {
                    wb = new XSSFWorkbook(new FileStream(file.FilePath, FileMode.Open));
                }
                else
                {
                    wb = new XSSFWorkbook();
                }
                this.Evaluator = new XSSFFormulaEvaluator(wb);
            }
            else
            {
                throw new System.Exception("无效的Excel扩展名");
            }
            return wb;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public ExcelFactory PreDocumentDescription<T>()
        {
            this.Context = new NOPIContext() { WorkBook = this.Workbook, Evaluator = this.Evaluator, Descripter = new DocumentDescription(typeof(T)), TitleSytle = this.TitleRender };
            this.InitSheet();
            return this;
        }

        public ExcelFactory PreDocumentDescription<T>(List<CellDescriptionAttribute> attributes)
        {
            var descripter = new DocumentDescription(typeof(T));
            descripter.AddCellDescription(attributes);

            this.Context = new NOPIContext()
            {
                WorkBook = this.Workbook,
                Evaluator = this.Evaluator,
                Descripter = descripter,
                TitleSytle = this.TitleRender
            };
            this.InitSheet();
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitSheet()
        {
            this.Sheet = new ActiveSheet(this.Context);
        }
        /// <summary>
        /// 
        /// </summary>
        public void CreateTitle()
        {
            this.Sheet.CreateTitleRow();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Attach<T>(List<T> datas)
        {
            datas.ForEach(data =>
            {
                this.Sheet.AttachRow(data);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetList<T>() where T : new()
        {
            this.PreDocumentDescription<T>();
            return this.GenericRender<T>(this.Sheet.GetRows()).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, string>> GetDictionary()
        {
            var sheet = this.Workbook.GetSheetAt(0);
            var titleDictionary = new Dictionary<int, string>();
            List<Dictionary<string, string>> valueDictionary = new List<Dictionary<string, string>>();

            var titleRowIndex = 0;
            var titleRow = sheet.GetRow(titleRowIndex);
            var cellIndex = 0;

            titleRow.Cells.ForEach(c =>
            {
                titleDictionary.Add(cellIndex, c.StringCellValue);
                cellIndex++;
            });

            var dataRows = sheet.GetRowEnumerator();
            var dataRowIndex = 0;
            while (dataRows.MoveNext())
            {
                if (dataRowIndex != 0)
                {
                    var value = new Dictionary<string, string>();
                    var sheetRow = (IRow)dataRows.Current;
                    if (sheetRow != null)
                    {
                        titleDictionary.ToList().ForEach(kv =>
                        {
                            var cell = sheetRow.GetCell(kv.Key);
                            if (cell != null)
                            {
                                string cellValue = "";
                                if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                                {
                                    cellValue = cell.NumericCellValue.ToString();
                                }
                                else
                                {
                                    cellValue = cell.StringCellValue;
                                }
                                value.Add(kv.Value, cellValue);
                            }
                            else
                            {
                                value.Add(kv.Value, null);
                            }
                        });
                        valueDictionary.Add(value);
                    }
                }
                dataRowIndex++;
            }
            return valueDictionary;
        }
        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<T> GenericRender<T>(List<ActiveRow> rows) where T : new()
        {
            var rowIndex = 1;
            var colIndex = 1;

            List<T> rowList = new List<T>();
            List<ListImportException> exceptionList = new List<ListImportException>();
            foreach (var row in rows)
            {
                try
                {
                    T obj = new T();
                    colIndex = 1;
                    obj.GetType().GetProperties().ToList().ForEach(p =>
                    {
                        var attr = (CellDescriptionAttribute)p.GetCustomAttribute(typeof(CellDescriptionAttribute));
                        if (attr != null)
                        {
                            var cell = row.Cells.Where(c => c.Description.Name == attr.Name).FirstOrDefault();
                            if (cell != null && cell.Value != null)
                            {
                                if (!p.PropertyType.IsGenericType)
                                {
                                    if (cell.CurrentCell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                                    {
                                        cell.Value = cell.Value == null || cell.Value.ToString() == "" ? 0 : cell.Value;
                                    }
                                    if (cell.CurrentCell.CellType == NPOI.SS.UserModel.CellType.Boolean)
                                    {
                                        cell.Value = cell.Value == null || cell.Value.ToString() == "" ? false : cell.Value;
                                    }
                                    p.SetValue(obj, Convert.ChangeType(cell.Value, p.PropertyType));
                                }
                                else
                                {
                                    if (cell.Value == null || cell.Value.ToString() == "")
                                    {
                                        cell.Value = null;
                                        p.SetValue(obj, cell.Value);
                                    }
                                    else
                                    {
                                        NullableConverter converter = new NullableConverter(p.PropertyType);
                                        var nullbaleType = converter.UnderlyingType;
                                        p.SetValue(obj, Convert.ChangeType(cell.Value, nullbaleType));
                                    }
                                }
                            }
                            colIndex++;
                        }
                    });
                    rowList.Add(obj);
                    rowIndex++;
                }
                catch (Exception ex)
                {
                    exceptionList.Add(new ListImportException(ex.Message) { Row = rowIndex, Cell = colIndex });
                    rowIndex++;
                }
            }
            if (exceptionList.Count > 0)
            {
                throw new ImportException(string.Format("第{0}行第{1}列产生异常,请检查excel！异常信息：{2}", exceptionList[0].Row, exceptionList[0].Cell, exceptionList[0].Message), exceptionList);
            }
            return rowList;
        }
        /// <summary>
        /// 
        /// </summary>
        public void GenerateFile()
        {
            FileStream stream = null;
            try
            {
                stream = this.FileManager.CreateStream();
                this.Workbook.Write(stream);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                this.Workbook.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void GenerateFile(string filePath)
        {
            FileManager manager = new FileManager(filePath);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                this.Workbook.Write(fileStream);
                this.Workbook.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public WeaveSheet GetWeaveSheet(string sheetName)
        {
            this.Context = new NOPIContext() { WorkBook = this.Workbook };
            DefaultCellStyle def = new DefaultCellStyle(this.Context);
            this.Context.CustomStyles.Add("table", def.DefaultTableStyle);
            this.Context.CustomStyles.Add("title", def.DefaultTitleStyle);
            this.Context.CustomStyles.Add("majorTitle", def.DefaultMajorTitleStyle);
            return new WeaveSheet(this.Context, sheetName);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WeaveSheet
    {
        public string[] cellIndex = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT","AU","AV","AW","AX","AY","AZ" };
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int IndexOf(string cell)
        {
            for (int i = 0; i < cellIndex.Length; i++)
            {
                if (cellIndex[i] == cell)
                {
                    return i;
                }
            }
            throw new Exception("未找到列映射");
        }
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, WeaveRow> Rows
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public NOPIContext Context
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ISheet CurrentSheet
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public WeaveSheet(NOPIContext context, string sheetName)
        {
            this.Context = context;
            this.CurrentSheet = this.Context.WorkBook.GetSheet(sheetName);
            if (this.CurrentSheet == null)
            {
                this.CurrentSheet = this.Context.WorkBook.CreateSheet(sheetName);
            }
            this.Rows = new Dictionary<int, WeaveRow>();
        }
        /// <summary>
        /// 
        /// </summary>
        //public void CreateBlank(int firstRow, int lastRow, string firstCellIndex, string lastCellIndex)
        //{
        //    if (firstRow < this.Rows.Count)
        //    {
        //        throw new Exception("必须在新行上创建空白行");
        //    }
        //    var createRows = lastRow - firstRow;
        //    for (int i = 0; i < createRows + 1; i++)
        //    {
        //        var weaveRow = new WeaveRow(this.Context, this.CurrentSheet, this.Rows.Count);
        //        this.Rows.Add(i, weaveRow);
        //    }
        //    this.CurrentSheet.AddMergedRegion(new CellRangeAddress(firstRow, lastRow, this.IndexOf(firstCellIndex), this.IndexOf(lastCellIndex)));
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public WeaveRow CreateRow(int rowIndex)
        {
            var row = new WeaveRow(this.Context, this.CurrentSheet, rowIndex);
            this.Rows.Add(rowIndex, row);
            return row;
        }
        /// <summary>
        /// 
        /// </summary>
        public void CreateRange(int firstRow, int lastRow, string firstCellIndex, string lastCellIndex, string value, string stylekey, short height = 20, int width = 10)
        {
            var fcIndex = this.IndexOf(firstCellIndex);
            var region = new CellRangeAddress(firstRow, lastRow, this.IndexOf(firstCellIndex), this.IndexOf(lastCellIndex));
            this.CurrentSheet.AddMergedRegion(region);

            for (int i = region.FirstRow; i <= region.LastRow; i++)
            {
                IRow row2 = this.CurrentSheet.GetRow(i);
                if (row2 != null)
                {
                    for (int j = region.FirstColumn; j <= region.LastColumn; j++)
                    {
                        ICell singleCell = row2.GetCell(j);
                        if (singleCell == null)
                        {
                            singleCell = row2.CreateCell(j);
                            this.CurrentSheet.SetColumnWidth(j, width * 256);
                        }
                        singleCell.CellStyle = this.Context.CustomStyles[stylekey];
                        if (fcIndex == j)
                        {
                            singleCell.SetCellValue(value);
                        }
                    }
                }
                else
                {
                    var weaveRow = this.CreateRow(i);
                    weaveRow.CurrentRow.HeightInPoints = (short)(height);
                    var row3 = weaveRow.CurrentRow;
                    for (int j = region.FirstColumn; j <= region.LastColumn; j++)
                    {
                        ICell singleCell = row3.GetCell(j);
                        if (singleCell == null)
                        {
                            singleCell = weaveRow.CreateCell(j).CurrentCell;
                            this.CurrentSheet.SetColumnWidth(j, width * 256);
                        }
                        singleCell.CellStyle = this.Context.CustomStyles[stylekey];
                        if (fcIndex == j)
                        {
                            singleCell.SetCellValue(value);
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DefaultCellStyle
    {
        public NOPIContext Context
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ICellStyle DefaultTableStyle
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ICellStyle DefaultTitleStyle
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ICellStyle DefaultMajorTitleStyle
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public DefaultCellStyle(NOPIContext context)
        {
            this.Context = context;
            this.DefaultTableStyleInit();
            this.DefaultTitleStyleInit();
            this.DefaultMajorTitleStyleInit();
        }
        /// <summary>
        /// 
        /// </summary>
        private void DefaultTitleStyleInit()
        {
            this.DefaultTitleStyle = this.Context.WorkBook.CreateCellStyle();

            //字体
            var font = this.Context.WorkBook.CreateFont();
            font.FontName = "宋体";
            font.FontHeightInPoints = 12;
            font.IsBold = true;
            this.DefaultTitleStyle.SetFont(font);

            this.DefaultTitleStyle.Alignment = HorizontalAlignment.Left;
            this.DefaultTitleStyle.VerticalAlignment = VerticalAlignment.Center;
        }
        /// <summary>
        /// 
        /// </summary>
        private void DefaultMajorTitleStyleInit()
        {
            this.DefaultMajorTitleStyle = this.Context.WorkBook.CreateCellStyle();

            //字体
            var font = this.Context.WorkBook.CreateFont();
            font.FontName = "宋体";
            font.FontHeightInPoints = 20;
            font.IsBold = true;
            this.DefaultMajorTitleStyle.SetFont(font);

            this.DefaultMajorTitleStyle.Alignment = HorizontalAlignment.Center;
            this.DefaultMajorTitleStyle.VerticalAlignment = VerticalAlignment.Center;
        }
        /// <summary>
        /// 
        /// </summary>
        private void DefaultTableStyleInit()
        {
            this.DefaultTableStyle = this.Context.WorkBook.CreateCellStyle();
            //边框
            this.DefaultTableStyle.BorderBottom = BorderStyle.Thin;
            this.DefaultTableStyle.BorderLeft = BorderStyle.Thin;
            this.DefaultTableStyle.BorderRight = BorderStyle.Thin;
            this.DefaultTableStyle.BorderTop = BorderStyle.Thin;

            //字体
            var font = this.Context.WorkBook.CreateFont();
            font.FontName = "宋体";
            font.FontHeightInPoints = 10;
            this.DefaultTableStyle.SetFont(font);

            this.DefaultTableStyle.Alignment = HorizontalAlignment.Center;
            this.DefaultTableStyle.VerticalAlignment = VerticalAlignment.Center;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WeaveRow
    {
        /// <summary>
        /// 
        /// </summary>
        public NOPIContext Context
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public IRow CurrentRow
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal Dictionary<int, WeaveCell> Cells
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        private ISheet CurrentSheet
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public WeaveRow(NOPIContext context, ISheet sheet, int rowIndex)
        {
            this.Context = context;
            this.CurrentSheet = sheet;
            this.CurrentRow = this.CurrentSheet.CreateRow(rowIndex);
            this.Cells = new Dictionary<int, WeaveCell>();
        }
        ///
        /// <summary>
        /// 
        /// </summary>
        public WeaveCell CreateCell(int cellIndex)
        {
            var cell = new WeaveCell(this.Context, this.CurrentRow, cellIndex);
            this.Cells.Add(cellIndex, cell);
            return cell;
        }
        /// <summary>
        /// 
        /// </summary>
        public WeaveCell CreateCell(int cellIndex, string styleKey)
        {
            var cell = new WeaveCell(this.Context, this.CurrentRow, cellIndex, this.Context.CustomStyles[styleKey]);
            this.Cells.Add(cellIndex, cell);
            return cell;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WeaveCell
    {
        /// <summary>
        /// 
        /// </summary>
        public NOPIContext Context
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public IRow CurrentRow
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ICell CurrentCell
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public WeaveCell(NOPIContext context, IRow row, int cellIndex, ICellStyle style)
        {
            this.Context = context;
            this.CurrentRow = row;
            this.CurrentCell = row.CreateCell(cellIndex);
            this.CurrentCell.CellStyle = style;
        }
        /// <summary>
        /// 
        /// </summary>
        public WeaveCell(NOPIContext context, IRow row, int cellIndex)
        {
            this.Context = context;
            this.CurrentRow = row;
            this.CurrentCell = row.CreateCell(cellIndex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(string value)
        {
            this.CurrentCell.SetCellValue(value);
        }
    }
    /// <summary>
    /// 文档属性描述
    /// </summary>
    internal class DocumentDescription
    {
        public SheetDescriptionAttribute sheetDescription
        {
            get; set;
        }
        public RowDescriptionAttribute rowDescription
        {
            get; set;
        }
        public List<CellDescriptionAttribute> cellDescription
        {
            get; set;
        }
        private Type ObjType
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        public DocumentDescription(Type objectType)
        {
            this.ObjType = objectType;
            this.InitSheetAndRowDescription();
            this.InitCellDescription();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitSheetAndRowDescription()
        {
            var attrs = this.ObjType.GetCustomAttributes(false);
            if (attrs != null)
            {
                foreach (var obj in attrs)
                {
                    if (obj is SheetDescriptionAttribute)
                    {
                        this.sheetDescription = obj as SheetDescriptionAttribute;
                    }
                    if (obj is RowDescriptionAttribute)
                    {
                        this.rowDescription = obj as RowDescriptionAttribute;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitCellDescription()
        {
            this.cellDescription = new List<CellDescriptionAttribute>();
            this.ObjType.GetProperties().ToList().ForEach(p =>
            {
                var attrs = p.GetCustomAttributes(typeof(CellDescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    var desc = attrs[0] as CellDescriptionAttribute;
                    desc.PropertyInfo = p;
                    desc.PropertyName = p.Name;
                    this.cellDescription.Add(desc);
                }
            });
            this.cellDescription = this.cellDescription.OrderBy(c => c.Order).ToList();
        }

        public void AddCellDescription(List<CellDescriptionAttribute> attributes)
        {
            if (attributes == null || attributes.Count == 0) return;

            if (this.cellDescription == null)
                this.cellDescription = new List<CellDescriptionAttribute>();

            attributes.ForEach(attribute =>
            {
                if (this.cellDescription.All(p => p.PropertyName != attribute.PropertyName))
                {
                    this.cellDescription.Add(attribute);
                }
            });
        }

        public void AddCellDescription(CellDescriptionAttribute attribute)
        {
            this.AddCellDescription(new List<CellDescriptionAttribute> { attribute });
        }
    }
    /// <summary>
    /// 当前活动sheet
    /// </summary>
    public class ActiveSheet
    {
        /// <summary>
        /// 
        /// </summary>
        public ISheet CurrentSheet
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public NOPIContext Context
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public List<ActiveRow> Rows
        {
            get; set;
        }
        /// <summary>
        /// 将会通过定义的特性在Excel中查找是否有已创建的sheet，否则创建一个新的sheet
        /// </summary>
        /// <param name="context"></param>
        public ActiveSheet(NOPIContext context)
        {
            this.Rows = new List<ActiveRow>();
            this.Context = context;
            this.Init();
        }
        /// <summary>
        /// 通过sheet名称创建一个新的sheet
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        public ActiveSheet(NOPIContext context, string name)
        {
            this.Rows = new List<ActiveRow>();
            this.Context = context;
            this.CurrentSheet = this.Context.WorkBook.CreateSheet(name);
        }
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            if (this.Context.Descripter.sheetDescription != null)
            {
                this.CurrentSheet = this.Context.WorkBook.GetSheet(this.Context.Descripter.sheetDescription.Name);
                if (this.CurrentSheet == null)
                {
                    this.CurrentSheet = this.Context.WorkBook.CreateSheet(this.Context.Descripter.sheetDescription.Name);
                    this.Context.Descripter.sheetDescription.OnSheetRender(this.CurrentSheet, this.Context);
                }
                else
                {
                    if (this.CurrentSheet.PhysicalNumberOfRows > 65533)
                    {
                        this.FindSheet(1);
                    }
                }
            }
            else
            {
                if (this.Context.WorkBook.NumberOfSheets == 0)
                {
                    this.CurrentSheet = this.Context.WorkBook.CreateSheet();
                }
                else
                {
                    this.CurrentSheet = this.Context.WorkBook.GetSheetAt(0);
                }
                //this.Context.Descripter.sheetDescription.OnSheetRender(this.CurrentSheet, this.Context);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void FindSheet(int index)
        {
            this.CurrentSheet = this.Context.WorkBook.GetSheet(this.Context.Descripter.sheetDescription.Name + index);
            if (this.CurrentSheet == null)
            {
                this.CurrentSheet = this.Context.WorkBook.CreateSheet(this.Context.Descripter.sheetDescription.Name + index);
                this.Context.Descripter.sheetDescription.OnSheetRender(this.CurrentSheet, this.Context);
            }
            else
            {
                if (this.CurrentSheet.PhysicalNumberOfRows > 65535)
                {
                    index++;
                    this.FindSheet(index);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void AttachRow(object data)
        {
            var row = new ActiveRow(this.Context, this.CurrentSheet, data);
            row.CreateCells();
            this.Rows.Add(row);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ActiveRow> GetRows()
        {
            List<ActiveRow> rows = new List<ActiveRow>();
            var map = new Dictionary<int, CellDescriptionAttribute>();
            var rowEnumerator = this.CurrentSheet.GetRowEnumerator();
            int index = 0;

            try
            {
                while (rowEnumerator.MoveNext())
                {
                    var sheetRow = (IRow)rowEnumerator.Current;
                    if (sheetRow != null)
                    {
                        if (index == 0)
                        {
                            ActiveRow row = new ActiveRow(this.Context, this.CurrentSheet, sheetRow, true);
                            map = row.GetCellDescriptionMap();
                        }
                        else
                        {
                            ActiveRow row = new ActiveRow(this.Context, this.CurrentSheet, sheetRow, false);
                            row.CellsInit(map);
                            rows.Add(row);
                        }
                        index++;
                    }
                }
            }
            catch (ImportException ex)
            {
                throw new ImportException(string.Format("第{0}行第{1}列产生异常,请检查excel", index + 1, ex.Message));
            }
            return rows;
        }
        /// <summary>
        /// 
        /// </summary>
        public void CreateTitleRow()
        {
            var row = new ActiveRow(this.Context, this.CurrentSheet, null);
            row.CreateTitle();
            this.Rows.Add(row);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ActiveRow
    {
        /// <summary>
        /// 
        /// </summary>
        public NOPIContext Context
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public IRow CurrentRow
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ISheet CurrentSheet
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public List<ActiveCell> Cells
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public object Data
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsTitleRow
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        public ActiveRow(NOPIContext context, ISheet sheet, object data)
        {
            this.Context = context;
            this.CurrentSheet = sheet;
            this.Data = data;
            this.Cells = new List<ActiveCell>();
            this.Context.Data = this.Data;
            this.Init();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sheet"></param>
        public ActiveRow(NOPIContext context, ISheet sheet, IRow row, bool IsTitleRow)
        {
            this.Context = context;
            this.CurrentSheet = sheet;
            this.CurrentRow = row;
            this.Cells = new List<ActiveCell>();
            this.IsTitleRow = IsTitleRow;
        }
        /// <summary>
        /// 
        /// </summary>
        public void CreateTitle()
        {
            if (this.Context.Descripter.cellDescription == null)
            {
                throw new Exception("缺少列描述,无法进行初始化！");
            }
            this.Context.Descripter.cellDescription.ForEach(desc =>
            {
                this.Cells.Add(new ActiveCell(this.Context, this.CurrentSheet, this.CurrentRow, desc.Name, desc, true));
            });
        }
        /// <summary>
        /// 
        /// </summary>
        public void CreateCells()
        {
            if (this.Context.Descripter.cellDescription == null)
            {
                throw new Exception("缺少列描述,无法进行初始化！");
            }

            this.Context.Descripter.cellDescription.ForEach(desc =>
            {
                object cellValue = null;
                if (Data is IDictionary dictionary && !string.IsNullOrEmpty(desc.PropertyName))
                {
                    cellValue = dictionary[desc.PropertyName];
                }
                else
                {
                    cellValue = desc.PropertyInfo.GetValue(Data);
                }

                this.Cells.Add(new ActiveCell(this.Context, this.CurrentSheet, this.CurrentRow, cellValue, desc));
            });
        }
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            this.CurrentRow = this.CurrentSheet.CreateRow(this.CurrentSheet.PhysicalNumberOfRows);
            if (this.Context.Descripter.rowDescription != null)
            {
                this.Context.Descripter.rowDescription.OnRowRender(this.CurrentRow, this.Context);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal void CellsInit(Dictionary<int, CellDescriptionAttribute> map)
        {
            var index = 0;
            try
            {
                map.ToList().ForEach(kv =>
                {
                    var cell = this.CurrentRow.GetCell(kv.Key);
                    var activeCell = new ActiveCell(this.Context, this.CurrentSheet, this.CurrentRow, cell, map[kv.Key], this.IsTitleRow);
                    this.Cells.Add(activeCell);
                });
            }
            catch
            {
                throw new ImportException(index.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, CellDescriptionAttribute> GetCellDescriptionMap()
        {
            if (this.IsTitleRow)
            {
                Dictionary<int, CellDescriptionAttribute> dicMap = new Dictionary<int, CellDescriptionAttribute>();
                int Index = 0;
                this.Context.TitleRow = this;
                this.CurrentRow.Cells.ForEach(c =>
                {
                    var description = this.Context.Descripter.cellDescription.Where(dc => dc.Name == c.StringCellValue).FirstOrDefault();
                    if (description != null)
                    {
                        dicMap.Add(Index, description);
                    }
                    Index++;
                });
                return dicMap;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public class ActiveCell
        {
            private Dictionary<NPOI.SS.UserModel.CellType, PropertyInfo> propertyMap = new Dictionary<NPOI.SS.UserModel.CellType, PropertyInfo>();
            /// <summary>
            /// 
            /// </summary>
            public NOPIContext Context
            {
                get;
                set;
            }
            /// <summary>
            /// 
            /// </summary>
            public ICell CurrentCell
            {
                get; set;
            }
            /// <summary>
            /// 
            /// </summary>
            public IRow CurrentRow
            {
                get; set;
            }
            /// <summary>
            /// 
            /// </summary>
            public ISheet CurrentSheet
            {
                get; set;
            }
            /// <summary>
            /// 
            /// </summary>
            public object Value
            {
                get; set;
            }
            /// <summary>
            /// 
            /// </summary>
            public CellDescriptionAttribute Description
            {
                get; set;
            }
            /// <summary>
            /// 
            /// </summary>
            public bool IsTitleCell
            {
                get; set;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="context"></param>
            /// <param name="sheet"></param>
            /// <param name="row"></param>
            /// <param name="value"></param>
            /// <param name="description"></param>
            public ActiveCell(NOPIContext context, ISheet sheet, IRow row, object value, CellDescriptionAttribute description, bool IsTitleCell = false)
            {
                this.Context = context;
                this.CurrentSheet = sheet;
                this.CurrentRow = row;
                this.Value = value;
                this.Description = description;
                this.IsTitleCell = IsTitleCell;
                this.Context.Value = this.Value;
                this.Init();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="context"></param>
            /// <param name="sheet"></param>
            /// <param name="row"></param>
            /// <param name="value"></param>
            /// <param name="description"></param>
            /// <param name="IsTitleCell"></param>
            public ActiveCell(NOPIContext context, ISheet sheet, IRow row, ICell cell, CellDescriptionAttribute description, bool IsTitleCell = false)
            {
                this.Context = context;
                this.CurrentSheet = sheet;
                this.CurrentRow = row;
                this.Description = description;
                this.IsTitleCell = IsTitleCell;
                this.CurrentCell = cell;
                if (this.CurrentCell != null)
                {
                    this.InitValueSetMap();
                    this.SetValue();
                }
            }
            /// <summary>
            /// 
            /// </summary>
            private void InitValueSetMap()
            {
                propertyMap.Add(NPOI.SS.UserModel.CellType.Numeric, this.CurrentCell.GetType().GetProperty("NumericCellValue"));
                propertyMap.Add(NPOI.SS.UserModel.CellType.String, this.CurrentCell.GetType().GetProperty("StringCellValue"));
                propertyMap.Add(NPOI.SS.UserModel.CellType.Boolean, this.CurrentCell.GetType().GetProperty("BooleanCellValue"));
            }
            /// <summary>
            /// 
            /// </summary>
            private void SetValue()
            {
                if (this.CurrentCell.CellType == NPOI.SS.UserModel.CellType.Formula)
                {
                    this.CurrentCell = this.Context.Evaluator.EvaluateInCell(this.CurrentCell);
                }
                if (this.propertyMap.ContainsKey(this.CurrentCell.CellType))
                {
                    if (this.CurrentCell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                    {
                        if (this.CurrentCell.CellStyle.DataFormat == 31 || this.CurrentCell.CellStyle.DataFormat == 165 || DateUtil.IsCellDateFormatted(this.CurrentCell))
                        {
                            this.Value = this.CurrentCell.DateCellValue;
                        }
                        else
                        {
                            this.Value = this.CurrentCell.NumericCellValue;
                        }
                    }
                    else
                    {
                        this.Value = this.propertyMap[this.CurrentCell.CellType].GetValue(this.CurrentCell);
                    }
                }
                else
                {
                    this.Value = this.CurrentCell.StringCellValue;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            private void Init()
            {
                if (this.Description == null)
                {
                    throw new Exception("缺少列描述，无法进行初始化！");
                }
                this.CurrentCell = this.CurrentRow.CreateCell(this.CurrentRow.LastCellNum == -1 ? 0 : this.CurrentRow.LastCellNum);
                if (this.IsTitleCell)
                {
                    this.Context.TitleSytle.Format(this.Context, this.CurrentCell);
                }
                else
                {
                    this.Description.OnCellRender(this.CurrentCell, this.Context);
                }
                this.CreateValue();
            }
            /// <summary>
            /// 
            /// </summary>
            public void CreateValue()
            {
                if (this.Value == null)
                {
                    this.CurrentCell.SetCellValue("");
                    return;
                }
                if (this.Value.GetType() == typeof(int))
                {
                    this.CurrentCell.SetCellValue((int)this.Value);
                }
                if (this.Value.GetType() == typeof(System.Int64))
                {
                    this.CurrentCell.SetCellValue(this.Value.ToString());
                }
                if (this.Value.GetType() == typeof(string))
                {
                    this.CurrentCell.SetCellValue(this.Value.ToString());
                }
                if (this.Value.GetType() == typeof(DateTime))
                {
                    this.CurrentCell.SetCellValue((DateTime)this.Value);
                }
                if (this.Value.GetType() == typeof(decimal))
                {
                    this.CurrentCell.SetCellValue(double.Parse(this.Value.ToString()));
                }
                if (this.Value.GetType() == typeof(double))
                {
                    this.CurrentCell.SetCellValue((double)this.Value);
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ImportException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ImportException(string message) : base(message)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptionList"></param>
        public ImportException(string message, List<ListImportException> exceptionList) : base(message)
        {
            this.ExceptionList = exceptionList;
        }
        /// <summary>
        /// 
        /// </summary>
        public List<ListImportException> ExceptionList
        {
            get; set;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ListImportException : Exception
    {
        public ListImportException(string message) : base(message)
        { }
        public int Row
        {
            get; set;
        }
        public int Cell
        {
            get; set;
        }
    }
}
