﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Areas.Admin.Models.Security
{
    public class SetupCode
    {
        public string Account { get; internal set; }
        public string AccountSecretKey { get; internal set; }
        public string ManualEntryKey { get; internal set; }
        public string QrCodeSetupImageUrl { get; internal set; }
    }
}