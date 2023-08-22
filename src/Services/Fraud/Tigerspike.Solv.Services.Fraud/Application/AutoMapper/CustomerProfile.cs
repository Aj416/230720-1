using AutoMapper;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Application.AutoMapper
{
	public class CustomerProfile : Profile
	{
		public CustomerProfile()
		{
			CreateMap<Customer, CustomerModel>()
				.ReverseMap();
		}
	}
}
