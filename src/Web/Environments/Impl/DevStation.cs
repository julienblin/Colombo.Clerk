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
using Castle.Facilities.Logging;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Colombo.Facilities;
using Colombo.TestSupport;

namespace Colombo.Clerk.Web.Environments.Impl
{
    public class DevStation : BaseEnvironment
    {
        public override void BootstrapContainer(IWindsorContainer container)
        {
            container.AddFacility<LoggingFacility>(f =>
            {
                f.LogUsing(LoggerImplementation.Log4net).WithConfig("log4net.config");
            });

            container.AddFacility<ColomboFacility>(f =>
            {
                f.DisableSendingThroughWcf();
                f.StatefulMessageBusLifestyle(typeof(PerWebRequestLifestyleManager));
            });

            RunInstallers(container);
            RegisterControllerFactory(container);

            container.Register(
                AllTypes.FromThisAssembly()
                    .Where(t => t.Namespace.Equals("Colombo.Clerk.Web.StubHandlers", StringComparison.InvariantCultureIgnoreCase))
                    .WithService.AllInterfaces()
            );
        }
    }
}