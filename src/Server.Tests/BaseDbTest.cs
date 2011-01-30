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

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Colombo.Clerk.Server.Models;
using Colombo.Facilities;
using Colombo.TestSupport;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace Colombo.Clerk.Server.Tests
{
    public abstract class BaseDbTest
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
        }

        protected IMessageBus MessageBus
        {
            get { return container.Resolve<IMessageBus>(); }
        }

        protected IStubMessageBus StubMessageBus
        {
            get { return container.Resolve<IStubMessageBus>(); }
        }

        protected ISession Session
        {
            get { return container.Resolve<ISession>(); }
        }

        protected ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.InMemory().ShowSql())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AuditEntryModel>())
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
