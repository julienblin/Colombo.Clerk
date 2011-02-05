using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Server.Services
{
    public interface IFullTextIndexer
    {
        void IndexAuditEntryModels();
    }
}
