﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 2023/6/18 16:27:47
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace EFTestModel
{
    public partial class TestChild {

        public TestChild()
        {
            OnCreated();
        }

        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }

        public virtual TestTable TestTable { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
