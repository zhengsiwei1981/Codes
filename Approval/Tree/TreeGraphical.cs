using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Tree
{
    public class TreeGraphical
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get;set;
        }
        [JsonProperty("value")]
        public string Value
        {
            get;set;
        }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("children")]
        public List<TreeGraphical> Children
        {
            get;set;
        }
        /// <summary>
        /// 
        /// </summary>
        public TreeGraphical()
        {
            this.Children = new List<TreeGraphical>();
        }
    }
}
