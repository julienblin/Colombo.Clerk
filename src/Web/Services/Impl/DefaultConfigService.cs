using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Web;

namespace Colombo.Clerk.Web.Services.Impl
{
    public class DefaultConfigService : IConfigService
    {
        public string Environment { get; private set; }

        public string ClerkServer
        {
            get
            {
                var channelEndPointElement = GetChannelEndpointElement("Colombo.Clerk.Messages");
                if (channelEndPointElement == null) return "Not configured";

                return channelEndPointElement.Address.ToString();
            }
        }

        private ChannelEndpointElement GetChannelEndpointElement(string endPointName)
        {
            return WcfConfigClientSection == null ?
                null : WcfConfigClientSection.Endpoints.Cast<ChannelEndpointElement>().FirstOrDefault(endPoint => endPoint.Name.Equals(endPointName, StringComparison.InvariantCultureIgnoreCase));
        }

        private ClientSection wcfConfigClientSection;

        private ClientSection WcfConfigClientSection
        {
            get
            {
                return wcfConfigClientSection ??
                       (wcfConfigClientSection =
                        ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection);
            }
        }
    }
}