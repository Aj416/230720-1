using AutoMapper;
using DomainModels = Tigerspike.Solv.Domain.Models;
using CachedModels = Tigerspike.Solv.Infra.Data.Models.Cached;

namespace Tigerspike.Solv.Infra.Data.AutoMapper
{
	public class CachedProfile : Profile
	{
		public CachedProfile()
		{
			CreateMap<DomainModels.Brand, CachedModels.Brand>();
			CreateMap<DomainModels.BrandFormField, CachedModels.BrandFormField>()
				.ForMember(m => m.Type, x => x.MapFrom(i => i.Type.Name));
		}
	}
}