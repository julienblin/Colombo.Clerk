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
using System.ServiceModel;
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
