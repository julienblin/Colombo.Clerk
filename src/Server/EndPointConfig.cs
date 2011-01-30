using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Colombo.Clerk.Server.Mappings;
using Colombo.Clerk.Server.Models;
using Colombo.Facilities;
using Colombo.Host;
using FluentNHibernate.Cfg;
using NHibernate;

namespace Colombo.Clerk.Server
{
    public class EndPointConfig : IAmAnEndpoint,
                                  IWantToRegisterOtherComponents, IWantToBeNotifiedWhenStartAndStop
    {
        public static IKernel Kernel { get; private set; }

        public void RegisterOtherComponents(IWindsorContainer container)
        {
            container.Register(
                Component.For<ISessionFactory>()
                    .UsingFactoryMethod(CreateSessionFactory),
                Component.For<ISession>()
                    .LifeStyle.PerRequestHandling()
                    .UsingFactoryMethod(k =>
                                        {
                                            var session = k.Resolve<ISessionFactory>().OpenSession();
                                            session.FlushMode = FlushMode.Commit;
                                            return session;
                                        })
            );
        }

        public void Start(IWindsorContainer container)
        {
            AutoMapperConfiguration.Configure();
            Kernel = container.Kernel;
        }

        public void Stop(IWindsorContainer container)
        {
            
        }

        private static ISessionFactory CreateSessionFactory()
        {
            var cfg = new NHibernate.Cfg.Configuration();
            cfg.Configure();
            return Fluently.Configure(cfg)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AuditEntry>())
                .BuildSessionFactory();
        }
    }
}
