using System;
using System.Linq;
using AutoMapper;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Brand;
using Tigerspike.Solv.Application.Models.Induction;
using Tigerspike.Solv.Domain.Commands.Brand;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Induction;

namespace Tigerspike.Solv.Application.AutoMapper
{
	public class BrandProfile : Profile
	{
		public BrandProfile()
		{
			CreateMap<Brand, BrandModel>()
				.ForMember(x => x.HasEscalationFlow, x => x.Ignore());

			CreateMap<Brand, BrandPrintableModel>();

			CreateMap<Brand, BrandInviteModel>()
				.ForMember(x => x.Invited, x => x.Ignore());

			CreateMap<Brand, AdvocateBrandModel>(MemberList.None);

			CreateMap<CreateBrandModel, CreateBrandCommand>()
				.IgnoreAllPropertiesWithAnInaccessibleSetter();

			CreateMap<CreateBrandCommand, Brand>()
				.ForMember(x => x.Id, x => x.Ignore())
				.ForMember(x => x.CreatedDate, x => x.Ignore())
				.ForMember(x => x.ModifiedDate, x => x.Ignore())
				.ForMember(x => x.IsPractice, x => x.Ignore())
				.ForMember(x => x.BillingDetailsId, x => x.Ignore())
				.ForMember(x => x.PaymentAccountId, x => x.Ignore())
				.ForMember(x => x.BillingAgreementToken, x => x.Ignore())
				.ForMember(x => x.BillingAgreementId, x => x.Ignore())
				.ForMember(x => x.QuizId, x => x.Ignore())
				.ForMember(x => x.ProbingForm, x => x.Ignore())
				.ForMember(x => x.ProbingFormId, x => x.Ignore())
				.ForMember(x => x.PaymentRoute, x => x.Ignore())
				.ForMember(x => x.Advocates, x => x.Ignore())
				.ForMember(x => x.InductionSections, x => x.Ignore())
				.ForMember(x => x.AdvocateApplications, x => x.Ignore())
				.ForMember(x => x.TicketEscalationConfigs, x => x.Ignore())
				.ForMember(x => x.AbandonReasons, x => x.Ignore())
				.ForMember(x => x.Tags, x => x.Ignore())
				.ForMember(x => x.FormFields, x => x.Ignore())
				.ForMember(x => x.NotificationConfig, x => x.Ignore())
				.ForMember(x => x.Categories, x => x.Ignore())
				.ForMember(x => x.SposDescription, x => x.Ignore())
				.ForMember(x => x.CategoryDescription, x => x.Ignore())
				.ForMember(x => x.ValidTransferDescription , x => x.Ignore())
				.ForMember(x => x.SkipCustomerForm , x => x.Ignore())
				.ForMember(x => x.ProbingFormRedirectUrl , x => x.Ignore())
				.ForMember(x => x.DiagnosisDescription, x => x.Ignore());

			CreateMap<Brand, BrandInductionModel>()
				.ForMember(x => x.Sections, x => x.MapFrom(src => src.InductionSections.OrderBy(x => x.Order)));

			CreateMap<Section, SectionModel>()
				.ForMember(dst => dst.Items, opt => opt.MapFrom(src => src.SectionItems.OrderBy(x => x.Order)));

			CreateMap<SectionItem, SectionItemModel>()
				.ForMember(dst => dst.Viewed, opt => opt.Ignore());

			CreateMap<Infra.Data.Models.Cached.Brand, BrandPublicModel>()
				.ForMember(m => m.AdditionalFields, x => x.MapFrom(src => src.FormFields.OrderBy(x => x.Order)))
				.ForMember(m => m.RedirectUrl, x => x.MapFrom(src => src.AutoRedirectUrl));

			CreateMap<Infra.Data.Models.Cached.BrandFormField, BrandFieldModel>()
				.ForMember(m => m.Validation,
					x => x.MapFrom(src => !string.IsNullOrEmpty(src.Validation) ? src.Validation.Split(",", StringSplitOptions.RemoveEmptyEntries) : new string[] { }))
				.ForMember(m => m.Options,
					x => x.MapFrom(src => !string.IsNullOrEmpty(src.Options) ? src.Options.Split(",", StringSplitOptions.RemoveEmptyEntries) : new string[] { }));

			CreateMap<Category, CategoryModel>();
		}
	}
}