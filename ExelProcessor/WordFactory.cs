using GJS.Infrastructure.Utility.NOPI.Interface;
using GJS.Infrastructure.Utility.NOPIFactory;
using GJS.Infrastructure.Utility.NOPIFactory.Attribute;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GJS.Infrastructure.Utility.NOPIFactory
{
    public class DocumentFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wordPath"></param>
        /// <param name="pdfDir"></param>
        public  static void ConvertToPdf(string filePath, string pdfDir)
        {
            var sofficePath = System.Configuration.ConfigurationManager.AppSettings["SofficePath"];
            if (sofficePath == null || sofficePath == "")
            {
                throw new Exception("无效的libreOffice路径");
            }
            Common.WordConvertPDF(sofficePath, filePath, pdfDir);
        }
    }
    public class WordFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public XWPFDocument Document
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        private FileManager FileManager
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Descripter Descripter
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal object Current
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public List<WordTable> Tables
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public WordFactory(string filePath)
        {
            this.FileManager = new FileManager(filePath);
            this.Tables = new List<WordTable>();
            this.CreateDocument();
        }
        /// <summary>
        /// 
        /// </summary>
        private void CreateDocument()
        {
            if (!FileManager.IsWord)
            {
                throw new Exception("无效的扩展名");
            }
            this.Document = new XWPFDocument(this.FileManager.CreateStream());
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitTable()
        {
            this.Document.Tables.ToList().ForEach(t =>
            {
                WordTable table = new WordTable(t, this.Descripter, new TextFormater(), this.Document);
                this.Tables.Add(table);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        public void Write(string fileName)
        {
            FileManager fm = new FileManager(fileName);
            fm.CheckDirectory();

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                this.Document.Write(fs);
                this.Document.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void ConvertToPdf(string wordPath, string pdfDir)
        {
            this.Write(wordPath);
            var sofficePath = System.Configuration.ConfigurationManager.AppSettings["SofficePath"];
            if (sofficePath == null || sofficePath == "")
            {
                throw new Exception("无效的libreOffice路径");
            }
            Common.WordConvertPDF(sofficePath, wordPath, pdfDir);
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitDescription()
        {
            var attrs = this.Current.GetType().GetCustomAttributes(typeof(WordDescriptionAttribute), false);
            if (attrs.Count() > 0)
            {
                this.Descripter = new Descripter((WordDescriptionAttribute)attrs[0]);
            }
            else
            {
                this.Descripter = new Descripter(new WordDescriptionAttribute() { PlaceHolder = "$" });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="customReplace">自定义替换仅支持带.的数组类型的段落标记</param>
        public void Render(object obj, ICustomReplace customReplace = null)
        {
            this.Current = obj;
            this.InitDescription();
            this.InitTable();
            ParagraphTextSetter paraTextSetter = new ParagraphTextSetter(new TextFormater(), this.Descripter, this.Document);
            obj.GetType().GetProperties().ToList().ForEach(p =>
            {
                this.Tables.ForEach(t =>
                {
                    t.Match(p.Name);
                    t.FindMaps().ForEach(m =>
                    {
                        m.Replace(this.Current);
                    });

                    var tbDescList = t.FindTableDescription();
                    if (tbDescList.Count > 0)
                    {
                        tbDescList.ForEach(tbDesc =>
                        {
                            tbDesc.Render(p.Name, obj);
                        });
                    }
                });

                paraTextSetter.Replace(obj, p.Name, customReplace);
            });

        }
    }
    public class ParagraphTextSetter
    {
        /// <summary>
        /// 
        /// </summary>
        internal List<XWPFParagraph> DocumentParagraphs
        {
            get; set;
        }
        internal XWPFDocument Document
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal Descripter Descripter
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal TextFormater Formater
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal List<XWPFParagraph> Paragraphs
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formater"></param>
        /// <param name="descripter"></param>
        /// <param name="document"></param>
        public ParagraphTextSetter(TextFormater formater, Descripter descripter, XWPFDocument document)
        {
            this.Formater = formater;
            this.Descripter = descripter;
            this.Document = document;
            this.InitDocumentParagraphs();
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitDocumentParagraphs()
        {
            this.DocumentParagraphs = this.Document.Paragraphs.ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Replace(object obj, string name, ICustomReplace customReplace = null)
        {
            NameParagraphMap map = new NameParagraphMap(this.Formater);
            var index = 1;
            this.DocumentParagraphs.ForEach(p =>
            {
                if (p.Text.Contains(this.Descripter.Description.PlaceHolder + name + this.Descripter.Description.PlaceHolder))
                {
                    map.Descripter = this.Descripter;
                    map.Name = name;
                    map.Paragraph = p;
                    map.Replace(obj);
                }
                else
                {
                    if (customReplace != null)
                    {
                        customReplace.Replace(obj, name, this, p);
                    }
                    else
                    {
                        var tbColumn = this.FindTableDescription(obj, name, p);
                        if (tbColumn != null)
                        {
                            var list = this.CreateList(tbColumn.ListName, obj);
                            foreach (var run in tbColumn.Paragraph.Runs)
                            {
                                run.SetText("");
                            }
                            foreach (var subObj in list)
                            {
                                tbColumn.Paragraph.CreateRun().AddCarriageReturn();
                                tbColumn.Paragraph.CreateRun().SetText(index + ") ");
                                tbColumn.Paragraph.CreateRun().SetText(this.GetPropertyValue(subObj, tbColumn.ColumnName));
                                //非通用写法，需要重构
                                var subDataPro = this.GetSubProList(subObj);
                                if (subDataPro != null)
                                {
                                    var subObjList = this.CreateList(subDataPro.Name, subObj);
                                    var subIndex = 1;
                                    foreach (var subObjListItem in subObjList)
                                    {
                                        tbColumn.Paragraph.CreateRun().AddCarriageReturn();
                                        tbColumn.Paragraph.CreateRun().SetText("   " + index + "." + subIndex + ")");
                                        tbColumn.Paragraph.CreateRun().SetText(" " + this.GetPropertyValue(subObjListItem, tbColumn.ColumnName));
                                        subIndex++;
                                    }
                                }
                                index++;
                            }
                        }
                    }
                }
            });
        }
        /// <summary>
        /// 非通用写法，需要重构
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public PropertyInfo GetSubProList(object obj)
        {
            foreach (var p in obj.GetType().GetProperties().ToList())
            {
                if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    return p;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetPropertyValue(object obj, string name)
        {
            var pro1 = obj.GetType().GetProperty(name);
            string refValue1 = "";
            if (pro1 != null)
            {
                var proValue = pro1.GetValue(obj);
                if (proValue != null)
                {
                    refValue1 = proValue.ToString();
                }
                else
                {
                    refValue1 = "";
                }
            }
            return refValue1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public IList CreateList(string name, object current)
        {
            var p = current.GetType().GetProperty(name);
            if (p != null)
            {
                var list = p.GetValue(current);
                if (list != null && list is IList)
                {
                    return (IList)list;
                }
            }
            throw new Exception("未找到可用的迭代数据");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        public TableColumn FindTableDescription(object obj, string name, XWPFParagraph para)
        {
            if (para.Text.Where(c => c.ToString() == this.Descripter.Description.PlaceHolder).Count() == 2)
            {
                var placeHolderStartIndex = para.Text.IndexOf(this.Descripter.Description.PlaceHolder);
                var placeHolderEndIndex = para.Text.LastIndexOf(this.Descripter.Description.PlaceHolder);

                var placeHolderName = para.Text.Substring(placeHolderStartIndex + 1, placeHolderEndIndex - placeHolderStartIndex - 1);
                if (placeHolderName.Contains("."))
                {
                    var splitNames = placeHolderName.Split(new char[] { '.' });
                    if (splitNames[0] == name)
                    {
                        return new TableColumn() { ListName = splitNames[0], ColumnName = splitNames[1], Paragraph = para };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class NameParagraphMap
    {
        internal Descripter Descripter
        {
            get; set;
        }
        internal TextFormater Formater
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formater"></param>
        public NameParagraphMap(TextFormater formater)
        {
            this.Formater = formater;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get; set;
        }
        public string ListName
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string InnerAllPlaceHolderText
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public XWPFParagraph Paragraph
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Replace(object obj)
        {
            foreach (var kv in this.Formater.replaceActions.ToList())
            {
                if (Paragraph.Text.Contains(kv.Key))
                {
                    kv.Value(string.Format("{0}{1}{2}{3}", this.Descripter.Description.PlaceHolder, this.GetMapName(), this.Descripter.Description.PlaceHolder, kv.Key), this.GetPropertyValue(obj), Paragraph);
                    return;
                }
            }
            Paragraph.ReplaceText(string.Format("{0}{1}{2}", this.Descripter.Description.PlaceHolder, this.GetMapName(), this.Descripter.Description.PlaceHolder), this.GetPropertyValue(obj));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetReplaceText(object obj)
        {
            foreach (var kv in this.Formater.replaceActions.ToList())
            {
                if (InnerAllPlaceHolderText.Contains(kv.Key))
                {
                    return kv.Value(string.Format("{0}{1}{2}{3}", this.Descripter.Description.PlaceHolder, this.GetMapName(), this.Descripter.Description.PlaceHolder, kv.Key), this.GetPropertyValue(obj), Paragraph);
                }
            }
            return this.GetPropertyValue(obj);
            //return string.Format("{0}{1}{2}", this.Descripter.Description.PlaceHolder, this.GetMapName(), this.Descripter.Description.PlaceHolder, this.GetPropertyValue(obj));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string GetPropertyValue(object obj)
        {
            var pro1 = obj.GetType().GetProperty(this.Name);
            string refValue1 = "";
            if (pro1 != null)
            {
                var proValue = pro1.GetValue(obj);
                if (proValue != null)
                {
                    refValue1 = proValue.ToString();
                }
                else
                {
                    refValue1 = "";
                }
            }
            return refValue1;
        }
        /// <summary>
        /// 
        /// </summary>
        private string GetMapName()
        {
            if (this.ListName != null && this.ListName.Length > 0)
            {
                return this.ListName + "." + this.Name;
            }
            else
            {
                return this.Name;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class TextFormater
    {
        public Dictionary<string, Func<string, string, XWPFParagraph, string>> replaceActions = new Dictionary<string, Func<string, string, XWPFParagraph, string>>();
        public TextFormater()
        {
            this.InitActions();
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitActions()
        {
            replaceActions.Add("[General]", (oldText, newText, p) =>
            {
                if (newText == null || newText == "")
                {
                    newText = "0";
                }
                var newValue = Common.GetDollorStr((double)Convert.ChangeType(newText, typeof(double)));
                if (p != null)
                {
                    p.ReplaceText(oldText, newValue);
                }
                return newValue;
            });
            replaceActions.Add("[Double]", (oldText, newText, p) =>
            {
                if (newText == null || newText == "")
                {
                    newText = "0";
                }
                var newValue = double.Parse(newText).ToString();
                if (p != null)
                {
                    p.ReplaceText(oldText, newValue);
                }
                return newValue;
            });
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Descripter
    {
        /// <summary>
        /// 
        /// </summary>
        public WordDescriptionAttribute Description
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Descripter(WordDescriptionAttribute attr)
        {
            this.Description = attr;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WordTable
    {
        public XWPFDocument Document
        {
            get; set;
        }
        public TextFormater Formater
        {
            get; set;
        }
        public Descripter Descripter
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public XWPFTable Table
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public List<WordRow> Rows
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        private List<string> MatchPropertyNames
        {
            get; set;
        }
        public WordTable(XWPFTable table, Descripter desc, TextFormater formater, XWPFDocument document)
        {
            this.Descripter = desc;
            this.MatchPropertyNames = new List<string>();
            this.Table = table;
            this.Formater = formater;
            this.Document = document;
            this.InitRows();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Match(string propertyName)
        {
            var hasName = this.Table.Text.Contains(this.Descripter.Description.PlaceHolder + propertyName + this.Descripter.Description.PlaceHolder);
            this.MatchPropertyNames.Add(propertyName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<NameParagraphMap> FindMaps()
        {
            List<NameParagraphMap> maps = new List<NameParagraphMap>();
            this.MatchPropertyNames.ForEach(name =>
            {
                this.Rows.ForEach(r =>
                {
                    var para = r.FindParagraph(name);
                    if (para != null)
                    {
                        maps.Add(new NameParagraphMap(this.Formater) { Name = name, Paragraph = para, Descripter = this.Descripter });
                    }
                });
            });
            return maps;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TableDescription> FindTableDescription()
        {
            var tbDescList = new List<TableDescription>();
            var index = 0;
            this.Rows.ForEach(r =>
            {
                var tb = r.FindTableDescription();
                if (tb != null)
                {
                    tb.Index = index;
                    tbDescList.Add(tb);
                }
                index++;
            });
            return tbDescList;
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitRows()
        {
            this.Rows = new List<WordRow>();
            this.Table.Rows.ForEach(r =>
            {
                this.Rows.Add(new WordRow(this.Descripter, r, this.Document) { Table = this.Table });
            });
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WordRow
    {
        public XWPFDocument Document
        {
            get; set;
        }
        public Descripter Descripter
        {
            get; set;
        }
        public XWPFTable Table
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public XWPFTableRow Row
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public List<WordCell> Cells
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public WordRow(Descripter desc, XWPFTableRow row, XWPFDocument document)
        {
            this.Document = document;
            this.Row = row;
            this.Descripter = desc;
            this.InitCells();
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitCells()
        {
            this.Cells = new List<WordCell>();
            this.Row.GetTableCells().ForEach(c =>
            {
                this.Cells.Add(new WordCell(this.Descripter, c, this.Document));
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public XWPFParagraph FindParagraph(string name)
        {
            foreach (var cell in this.Cells)
            {
                var p = cell.FindParagraph(name);
                if (p != null)
                {
                    return p;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TableDescription FindTableDescription()
        {
            TableDescription tbDesc = null;
            foreach (var cell in this.Cells)
            {
                var columnDesc = cell.FindTableDescriptionParagraph();
                if (columnDesc != null)
                {
                    if (tbDesc == null)
                    {
                        tbDesc = new TableDescription(this.Row, this.Document);
                        tbDesc.ListName = columnDesc.ListName;
                        tbDesc.Descripter = this.Descripter;
                        tbDesc.Table = this.Table;
                    }
                    tbDesc.Columns.Add(columnDesc);
                }
            }
            return tbDesc;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WordCell
    {
        public XWPFDocument Document
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public WordCell(Descripter desc, XWPFTableCell cell, XWPFDocument document)
        {
            this.Cell = cell;
            this.Descripter = desc;
            this.InitParagraphs();
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitParagraphs()
        {
            this.Paragraphs = this.Cell.Paragraphs.ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        public Descripter Descripter
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public XWPFTableCell Cell
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public List<XWPFParagraph> Paragraphs
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XWPFParagraph FindParagraph(string name)
        {
            return this.Paragraphs.Where(p => p.Text.Contains(string.Format("{0}{1}{2}", this.Descripter.Description.PlaceHolder, name, this.Descripter.Description.PlaceHolder))).FirstOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TableColumn FindTableDescriptionParagraph()
        {
            foreach (var para in this.Paragraphs)
            {
                if (para.Text.Where(c => c.ToString() == this.Descripter.Description.PlaceHolder).Count() == 2)
                {
                    var placeHolderStartIndex = para.Text.IndexOf(this.Descripter.Description.PlaceHolder);
                    var placeHolderEndIndex = para.Text.LastIndexOf(this.Descripter.Description.PlaceHolder);

                    var placeHolderName = para.Text.Substring(placeHolderStartIndex + 1, placeHolderEndIndex - placeHolderStartIndex - 1);
                    if (placeHolderName.Contains("."))
                    {
                        var splitNames = placeHolderName.Split(new char[] { '.' });
                        return new TableColumn() { ListName = splitNames[0], ColumnName = splitNames[1], Paragraph = para, Cell = this.Cell };
                    }
                }
            }
            return null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class TableDescription
    {
        /// <summary>
        /// 
        /// </summary>
        public XWPFDocument Document
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Descripter Descripter
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string ListName
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public List<TableColumn> Columns
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public XWPFTableRow TargetRow
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Index
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public XWPFTable Table
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetRow"></param>
        public TableDescription(XWPFTableRow targetRow, XWPFDocument document)
        {
            this.TargetRow = targetRow;
            this.Document = document;
            this.Table = this.TargetRow.GetTable();
            this.Columns = new List<TableColumn>();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Render(string name, object current)
        {
            List<XWPFTableRow> rowList = new List<XWPFTableRow>();
            if (!name.Equals(this.ListName))
            {
                return;
            }
            var list = this.CreateList(name, current);
            var index = this.Index;
            if (list.Count == 0)
            {
                this.Table.RemoveRow(index);
                return;
            }
            bool hasRow = true;
            while (hasRow)
            {
                var row = this.Table.GetRow(index + 1);
                hasRow = row != null;
                if (hasRow)
                {
                    rowList.Add(row);
                    this.Table.RemoveRow(index + 1);
                }
            }

            for (int i = 1; i < list.Count; i++)
            {
                CT_Row nr = new CT_Row();
                var row = new XWPFTableRow(nr, this.Table);//创建行
                this.Table.AddRow(row);

                this.Columns.ForEach(c =>
                {
                    var cellCount = this.TargetRow.GetTableCells().Count;
                    for (int l = 0; l < cellCount; l++)
                    {
                        var cell = this.TargetRow.GetCell(l);
                        if (c.Cell.GetText() == cell.GetText())
                        {
                            NameParagraphMap map = new NameParagraphMap(new TextFormater());
                            map.Descripter = this.Descripter;
                            map.Name = c.ColumnName;
                            map.ListName = c.ListName;
                            map.InnerAllPlaceHolderText = c.Cell.GetText();

                            var newCell = row.CreateCell();

                            newCell.RemoveParagraph(0);
                            var para = newCell.AddParagraph();
                            para.Alignment = ParagraphAlignment.CENTER;
                            row.Height = 10;
                            var run = para.CreateRun();
                            run.FontSize = 9;

                            run.SetText(map.GetReplaceText(list[i]));
                            //newCell.SetParagraph(para);
                            //newCell.SetText(map.GetReplaceText(list[i]));
                            newCell.SetVerticalAlignment(XWPFTableCell.XWPFVertAlign.CENTER);
                        }
                    }
                });
            }

            rowList.ForEach(r =>
            {
                this.Table.AddRow(r);
            });

            if (list.Count > 0)
            {
                this.Columns.ForEach(c =>
                {
                    this.Replace(this.TargetRow, list[0], c.Paragraph, c.ColumnName, c.ListName);
                });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        private void Replace(XWPFTableRow row, object obj, XWPFParagraph para, string name, string listName)
        {
            NameParagraphMap map = new NameParagraphMap(new TextFormater());
            map.Descripter = this.Descripter;
            map.Paragraph = para;
            map.Name = name;
            map.ListName = listName;
            map.Replace(obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private IList CreateList(string name, object current)
        {
            var p = current.GetType().GetProperty(name);
            if (p != null)
            {
                var list = p.GetValue(current);
                if (list != null && list is IList)
                {
                    return (IList)list;
                }
            }
            throw new Exception("未找到可用的迭代数据");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class TableColumn
    {
        public string ListName
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string ColumnName
        {
            get; set;
        }
        public XWPFTableCell Cell
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public XWPFParagraph Paragraph
        {
            get; set;
        }
    }
}
