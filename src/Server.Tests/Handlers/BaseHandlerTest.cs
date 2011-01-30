using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Colombo.Clerk.Server.Mappings;
using Colombo.Clerk.Server.Models;
using Colombo.Facilities;
using Colombo.TestSupport;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace Colombo.Clerk.Server.Tests.Handlers
{
    public abstract class BaseHandlerTest
    {
        protected IWindsorContainer container;
        protected Configuration nhConfig;

        [SetUp]
        public void SetUp()
        {
            container = new WindsorContainer();
            container.AddFacility<ColomboFacility>(f => f.TestSupportMode());

            container.Register(
                Component.For<ISessionFactory>()
                    .UsingFactoryMethod(CreateSessionFactory),
                Component.For<ISession>()
                    .UsingFactoryMethod(k =>
                                        {
                                            var session = k.Resolve<ISessionFactory>().OpenSession();
                                            session.FlushMode = FlushMode.Commit;
                                            return session;
                                        })
            );

            BuildSchema();

            AutoMapperConfiguration.Configure();
        }

        private IMessageBus messageBus;

        protected IMessageBus MessageBus
        {
            get { return messageBus ?? (messageBus = container.Resolve<IMessageBus>()); }
        }

        private IStubMessageBus stubMessageBus;

        protected IStubMessageBus StubMessageBus
        {
            get { return stubMessageBus ?? (stubMessageBus = container.Resolve<IStubMessageBus>()); }
        }

        protected ISession Session
        {
            get { return container.Resolve<ISession>(); }
        }

        protected ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.InMemory().ShowSql())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AuditEntry>())
                .ExposeConfiguration(cfg => nhConfig = cfg)
                .BuildSessionFactory();
        }

        protected void BuildSchema()
        {
            var session = container.Resolve<ISession>();

            using (var tx = session.BeginTransaction())
            {
                var export = new SchemaExport(nhConfig);
                export.Execute(true, true, false, session.Connection, null);
                tx.Commit();
            }
        }
    }
}
