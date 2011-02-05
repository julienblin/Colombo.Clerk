#region License
// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Colombo.Clerk.Server.Models;
using Colombo.Clerk.Server.Services;
using Colombo.Clerk.Server.Services.Impl;
using Colombo.Facilities;
using Colombo.Host;
using Colombo.Wcf;
using FluentNHibernate.Cfg;
using Lucene.Net.Analysis;
using NHibernate;
using NHibernate.Event;
using NHibernate.Search;
using NHibernate.Search.Event;
using NHibernate.Search.Store;
using NHibernate.Tool.hbm2ddl;
using Configuration = NHibernate.Cfg.Configuration;

namespace Colombo.Clerk.Server
{
    public class EndPointConfig : IAmAnEndpoint,
                                  IWantToConfigureLogging,
                                  IWantToRegisterOtherComponents,
                                  IWantToCreateServiceHosts,
                                  IWantToBeNotifiedWhenStartAndStop
    {
        public static IKernel Kernel { get; internal set; }

        public const string FullTextSearchIndexDirectoryAppSettingsKey = @"FullTextSearchIndexDirectory";

        public static string FullTextSearchIndexDirectory { get; private set; }

        private bool mustReindex = false;

        private ILogger logger = NullLogger.Instance;
        /// <summary>
        /// Logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        public void ConfigureLogging(IWindsorContainer container)
        {
            container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.Log4net).WithConfig("log4net.config"));
            Logger = container.Resolve<ILogger>();
        }

        public void RegisterOtherComponents(IWindsorContainer container)
        {
            container.Register(
                Component.For<ISessionFactory>()
                    .UsingFactoryMethod(CreateSessionFactory),
                Component.For<ISession>()
                    .LifeStyle.PerRequestHandling()
                    .UsingFactoryMethod(k => k.Resolve<ISessionFactory>().OpenSession())
            );

            FullTextSearchIndexDirectory = ConfigurationManager.AppSettings[FullTextSearchIndexDirectoryAppSettingsKey];
            if (FullTextSearchIndexDirectory != null)
            {
                FullTextSearchIndexDirectory = FullTextSearchIndexDirectory.Replace("~",
                                                                                    AppDomain.CurrentDomain.
                                                                                        BaseDirectory);
                Logger.InfoFormat("Full text search enabled using directory {0} to store index.", FullTextSearchIndexDirectory);
                container.Register(
                    Component.For<IFullTextSession>()
                        .LifeStyle.PerRequestHandling()
                        .UsingFactoryMethod(k => Search.CreateFullTextSession(k.Resolve<ISession>())),
                    Component.For<IFullTextIndexer>()
                        .LifeStyle.Transient
                        .ImplementedBy<DefaultFullTextIndexer>()
                );

                var indexDirInfo = new DirectoryInfo(FullTextSearchIndexDirectory);
                if (!indexDirInfo.Exists || ((indexDirInfo.GetFiles().Count() + indexDirInfo.GetDirectories().Count()) == 0))
                {
                    mustReindex = true;
                }
                else
                {
                    logger.InfoFormat("Directory {0} exists and is not empty. No initial indexing will be executed. To force a re-index, delete everything inside the index directory and restart the server.",
                        FullTextSearchIndexDirectory);
                }
            }
            else
            {
                Logger.InfoFormat("Full text search disabled. Add an appSetting with key=\"{0}\" and value=\"<a writable directory to store index>\" in {1}.dll.config to enable full text search.",
                    FullTextSearchIndexDirectoryAppSettingsKey,
                    typeof(EndPointConfig).Assembly.GetName().Name);
            }
        }

        public IEnumerable<ServiceHost> CreateServiceHosts(IWindsorContainer container)
        {
            foreach (var contract in from ServiceElement serviceElement in WcfConfigServicesSection.Services
                                     where serviceElement.Endpoints.Count > 0
                                     select serviceElement.Endpoints[0].Contract)
            {
                switch (contract)
                {
                    case "Colombo.Wcf.IColomboService":
                        yield return new ServiceHost(typeof(ColomboService));
                        break;
                    case "Colombo.Wcf.ISoapService":
                        yield return new ServiceHost(typeof(SoapService));
                        break;
                    case "Colombo.Clerk.Service.IClerkService":
                        yield return new ServiceHost(typeof(ClerkService));
                        break;
                    default:
                        throw new ColomboException(string.Format("Unrecognized contract {0}.", contract));
                }
            }
        }

        public void Start(IWindsorContainer container)
        {
            Kernel = container.Kernel;

            try
            {
                container.Resolve<ISessionFactory>();

                if (container.Kernel.HasComponent(typeof(IFullTextIndexer)))
                {
                    if (mustReindex)
                        Task.Factory.StartNew(() => container.Resolve<IFullTextIndexer>().IndexAuditEntryModels());
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error while configuring database connection. Check the database.config file.", ex);
                throw;
            }
        }

        public void Stop(IWindsorContainer container)
        {

        }

        private static ISessionFactory CreateSessionFactory()
        {
            var cfg = new Configuration().Configure("database.config");
            var byteCodeType = Type.GetType("NHibernate.ByteCode.Castle.ProxyFactoryFactory");
            if (byteCodeType == null)
            {
                // When not ILMerged.
                cfg.SetProperty("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
            }
            else
            {
                // When ILMerged after release process.
                cfg.SetProperty("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory");
            }
            var fluentConfig = Fluently.Configure(cfg)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AuditEntryModel>())
                .ExposeConfiguration(config =>
                                     {
                                         try
                                         {
                                             var schemaValidator = new SchemaValidator(config);
                                             schemaValidator.Validate();

                                             if (FullTextSearchIndexDirectory != null)
                                             {
                                                 cfg.SetListener(ListenerType.PostInsert, new FullTextIndexEventListener());
                                                 cfg.SetListener(ListenerType.PostUpdate, new FullTextIndexEventListener());
                                                 cfg.SetListener(ListenerType.PostDelete, new FullTextIndexEventListener());
                                                 cfg.SetProperty(NHibernate.Search.Environment.AnalyzerClass,
                                                                 typeof(SimpleAnalyzer).AssemblyQualifiedName);
                                                 cfg.SetProperty("hibernate.search.default.directory_provider",
                                                                 typeof(FSDirectoryProvider).AssemblyQualifiedName);
                                                 cfg.SetProperty("hibernate.search.default.indexBase", FullTextSearchIndexDirectory);
                                             }
                                         }
                                         catch (HibernateException hibernateException)
                                         {
                                             throw new ColomboException("Error while verifying database schema. Did you create the mandatory tables correctly?", hibernateException);
                                         }
                                     });

            return fluentConfig.BuildSessionFactory();
        }

        private ServicesSection wcfConfigServicesSection;

        private ServicesSection WcfConfigServicesSection
        {
            get
            {
                if (wcfConfigServicesSection == null)
                {
                    var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    var serviceModelGroup = ServiceModelSectionGroup.GetSectionGroup(configuration);
                    if (serviceModelGroup != null)
                        wcfConfigServicesSection = serviceModelGroup.Services;
                }
                return wcfConfigServicesSection;
            }
        }
    }
}
