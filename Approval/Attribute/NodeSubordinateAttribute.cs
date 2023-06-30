using System;
using GJS.Infrastructure.Enum;
using System.Collections.Generic;

namespace GJS.Service.Approval.NodeAttribute
{
    [AttributeUsage( AttributeTargets.Class)]
    public class NodeSubordinateAttribute : Attribute
    {
        public NodeSubordinateEnum[] Subordinate
        {
            get; set;
        }
        public NodeSubordinateAttribute(params NodeSubordinateEnum [] subordinates)
        {
            this.Subordinate = subordinates;
        }
    }
}
