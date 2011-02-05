using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Colombo.Clerk.Server.Models;
using NHibernate;
using NHibernate.Search;
using NHibernate.Transform;

namespace Colombo.Clerk.Server.Services.Impl
{
    public class DefaultFullTextIndexer : IFullTextIndexer
    {
        public const int PageSize = 100;

        private ILogger logger = NullLogger.Instance;
        /// <summary>
        /// Logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        public ISession Session { get; set; }

        public void IndexAuditEntryModels()
        {
            try
            {
                var auditEntriesCount = Session.QueryOver<AuditEntryModel>().RowCount();
                Logger.InfoFormat("Start indexing of {0} audit entries...", auditEntriesCount);
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                using (var fullTextSession = Search.CreateFullTextSession(Session))
                {
                    var currentPage = 0;
                    var auditEntries = Session.QueryOver<AuditEntryModel>()
                                        //.Fetch(x => x.Context).Eager
                                        .Take(PageSize)
                                        .Skip(currentPage * PageSize)
                                        .List();
                    while (auditEntries.Count > 0)
                    {
                        Logger.InfoFormat("\nAuditEntries.Count = {0}\n", auditEntries.Count);
                        foreach (var auditEntryModel in auditEntries)
                        {
                            fullTextSession.Index(auditEntryModel);
                        }
                        var percentDone = (((currentPage * PageSize) + auditEntries.Count) / Convert.ToDouble(auditEntriesCount)) * 100.0;
                        Logger.DebugFormat("Indexed {0}%", Convert.ToInt32(percentDone));
                        ++currentPage;
                        auditEntries = Session.QueryOver<AuditEntryModel>()
                                        //.Fetch(x => x.Context).Eager
                                        .Take(PageSize)
                                        .Skip(currentPage * PageSize)
                                        .List();
                    }
                }
                stopWatch.Stop();
                var ts = stopWatch.Elapsed;
                Logger.InfoFormat("Indexed {0} audit entries in {1:00}:{2:00}:{3:00}.{4:00}. Average time per entry: {5}ms.", auditEntriesCount,
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10, auditEntriesCount / stopWatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                logger.Error("Error while creating full text index.", ex);
            }
        }
    }
}
