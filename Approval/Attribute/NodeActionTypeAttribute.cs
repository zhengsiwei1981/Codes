using GJS.Infrastructure.Enum;
using System;

namespace GJS.Service.Approval.NodeAttribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeActionTypeAttribute : Attribute
    {
        public NodeTypeEnum Type
        {
            get; set;
        }
        public NodeActionTypeAttribute(NodeTypeEnum type)
        {
            this.Type = type;
        }
    }
}
