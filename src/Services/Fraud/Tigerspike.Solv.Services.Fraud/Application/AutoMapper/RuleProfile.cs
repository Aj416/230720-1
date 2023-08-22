using System;
using AutoMapper;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Application.AutoMapper
{
	public class RuleProfile : Profile
	{
		public RuleProfile()
		{
			CreateMap<Rule, RuleModel>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
				.ReverseMap();
		}
	}
}