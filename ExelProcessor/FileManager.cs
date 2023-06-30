using System;
using System.IO;
using System.Web.Hosting;

namespace GJS.Infrastructure.Utility.NOPIFactory
{
    public class FileManager
    {
        /// <summary>
        /// 
        /// </summary>
        public bool HasFile
        {
            get;set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Extension
        {
            get;set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string FilePath
        {
            get;set;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsExcel
        {
            get
            {
                return this.Extension == ".xls" || this.Extension == ".xlsx";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsWord
        {
            get
            {
                return this.Extension == ".docx" || this.Extension == ".doc";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public FileManager(string filePath)
        {
            this.FilePath = filePath;
            this.CheckDirectory();
            this.HasFile = File.Exists(filePath);
            this.Extension = Path.GetExtension(filePath);
        }
        /// <summary>
        /// 
        /// </summary>
        public void CheckDirectory()
        {
            var dirPath = Path.GetDirectoryName(this.FilePath);
            var dirInfo = new DirectoryInfo(dirPath);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FileStream CreateStream()
        {
            if (!HasFile)
            {
                return new FileStream(this.FilePath, FileMode.CreateNew);
            }
            else
            {
                return new FileStream(this.FilePath, FileMode.OpenOrCreate);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetAbsolutSiteFilePath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetStaticFileName(string title,string extension)
        {
            return title + (DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString()) + "." + extension;
;        }
    }
}
