using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Models;
using Colombo.Clerk.Service;

namespace Colombo.Clerk.Server.Mappings
{
    internal static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<AuditInfo, AuditEntry>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            Mapper.CreateMap<AuditEntry, AuditEntryResponse>()
                .ForMember(x => x.CorrelationGuid, opt => opt.Ignore())
                .ForMember(x => x.UtcTimestamp, opt => opt.Ignore())
                .ForMember(x => x.Found, opt => opt.UseValue(true))
                .ForMember(x => x.Request, opt => opt.MapFrom(x => new AuditEntryResponse.InnerInfo
                                                                       {
                                                                           Namespace = x.RequestNamespace,
                                                                           Type = x.RequestType,
                                                                           Serialized = x.RequestSerialized,
                                                                           CorrelationGuid = x.RequestCorrelationGuid
                                                                       }))
                .ForMember(x => x.Response, opt => opt.Ignore())
                .ForMember(x => x.Exception, opt => opt.Ignore())
                .ForMember(x => x.ServerMachineName, opt => opt.Ignore());
        }
    }
}
