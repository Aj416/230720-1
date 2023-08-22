using System;
using System.Linq;
using AutoMapper;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Enum;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Application.AutoMapper
{
	public class TicketProfile : Profile
	{
		public TicketProfile()
		{
			CreateMap<Ticket, TicketModel>()
				.ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => Guid.Parse(src.TicketId)))
				.ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => Guid.Parse(src.CustomerId)))
				.ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.CustomerDetail.IpAddress))
				.ForMember(dest => dest.Rules, opt => opt.Ignore())
				.ReverseMap();

			CreateMap<TicketModel, FraudSearchModel>()
				.ForMember(dest => dest.FraudStatus, opt => opt.MapFrom(src => src.FraudStatus))
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TicketId))
				.ForMember(dest => dest.AdvocateName, opt => opt.MapFrom(src => src.AssignedTo ?? string.Empty))
				.ForMember(dest => dest.FraudRisks, opt => opt.MapFrom(src => src.Rules
					.Select(rm => rm.Label)))
				.ForMember(dest => dest.FraudLevel, opt => opt.MapFrom(src => src.Rules
					.Select(x => (int?)x.Risk).OrderByDescending(x => x).FirstOrDefault()))
				.ForMember(dest => dest.Metadata, opt => opt.MapFrom<SearchMetadataResolver>())
				.ReverseMap();
		}
	}
}