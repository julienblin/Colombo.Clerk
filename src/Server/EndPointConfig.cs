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
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Colombo.Clerk.Server.Models;
using Colombo.Facilities;
using Colombo.Host;
using Colombo.Wcf;
using FluentNHibernate.Cfg;
using NHibernate;
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

        public void ConfigureLogging(IWindsorContainer container)
        {
            container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.Log4net).WithConfig("log4net.config"));
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
        }

        public void Stop(IWindsorContainer container)
        {
            
        }

        private static ISessionFactory CreateSessionFactory()
        {
            var cfg = new Configuration();
            cfg.Configure();
            return Fluently.Configure(cfg)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AuditEntryModel>())
                .BuildSessionFactory();
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
