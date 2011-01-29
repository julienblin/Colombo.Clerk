using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Castle.Core.Logging;
using Colombo.Clerk.Service;

namespace Colombo.Clerk.Client.Impl
{
    public class ClerkServiceFactory : IClerkServiceFactory
    {
        public const string EndPointName = @"Clerk";

        private ILogger logger = NullLogger.Instance;
        /// <summary>
        /// Logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        ChannelFactory<IClerkService> channelFactory;

        public object syncRoot = new object();

        public IClerkService CreateClerkService()
        {
            try
            {
                lock(syncRoot)
                {
                    if(channelFactory == null)
                    {
                        channelFactory = new ChannelFactory<IClerkService>(EndPointName);
                        channelFactory.Faulted += FactoryFaulted;
                        channelFactory.Open();
                    }
                }

                var channel = channelFactory.CreateChannel();
                ((IClientChannel)channel).Faulted += ChannelFaulted;
                return channel;
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to create a WCF Channel for Clerk. Did you create an endPoint for Colombo.Clerk.Service.IClerkService?", ex);
                return null;
            }
        }

        private void FactoryFaulted(object sender, EventArgs args)
        {
            var factory = (ChannelFactory<IClerkService>)sender;
            try
            {
                factory.Close();
            }
            catch
            {
                factory.Abort();
            }

            lock(syncRoot)
            {
                channelFactory = null;
            }
        }

        private static void ChannelFaulted(object sender, EventArgs e)
        {
            var channel = (IClientChannel)sender;
            try
            {
                channel.Close();
            }
            catch
            {
                channel.Abort();
            }
        }
    }
}
