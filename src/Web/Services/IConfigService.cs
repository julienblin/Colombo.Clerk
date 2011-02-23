﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Web.Services
{
    public interface IConfigService
    {
        string Environment { get; }

        bool FakeSend { get; }

        string ClerkServer { get; }
    }
}