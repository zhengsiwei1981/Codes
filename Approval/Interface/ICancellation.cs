﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJS.Service.Approval.Interface
{
    public interface ICancellation
    {
        void Cancel(ApprovalObject approvalObject);
    }
}
