using System;
using System.Linq;
using AutoMapper;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Client;
using Tigerspike.Solv.Application.Models.Profile;
using Tigerspike.Solv.Application.Models.Profiling;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Application.AutoMapper
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<User, UserModel>()
				.ForMember(x => x.Avatar, opt => opt.Ignore());

			CreateMap<Advocate, AdvocateModel>()
				.ForMember(m => m.Email, opt => opt.MapFrom(src => src.User.Email))
				.ForMember(m => m.Enabled, opt => opt.MapFrom(src => src.User.Enabled))
				.ForMember(m => m.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
				.ForMember(m => m.LastName, opt => opt.MapFrom(src => src.User.LastName))
				.ForMember(m => m.FullName, opt => opt.MapFrom(src => src.User.FullName))
				.ForMember(m => m.Phone, opt => opt.MapFrom(src => src.User.Phone))
				.ForMember(m => m.State, opt => opt.MapFrom(src => src.User.State))
				.ForMember(m => m.CreatedDate, opt => opt.MapFrom(src => src.User.CreatedDate))
				.ForMember(m => m.BlockHistory, opt => opt.MapFrom(src => src.BlockHistory.Select(b => new AdvocateBlockedSearchModel
				{
					BrandName = b.Brand.Name,
					BlockedDate = b.CreatedDate
				})))
				.ForMember(m => m.Csat, opt => opt.Ignore())
				.ForMember(m => m.ProfileStatus, opt => opt.MapFrom(src => src.ProfileStatus.ToString()));

			CreateMap<Advocate, AdvocatePrintableModel>()
				.ForMember(m => m.Email, opt => opt.MapFrom(src => src.User.Email))
				.ForMember(m => m.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
				.ForMember(m => m.LastName, opt => opt.MapFrom(src => src.User.LastName))
				.ForMember(m => m.Phone, opt => opt.MapFrom(src => src.User.Phone));

			CreateMap<Client, ClientModel>()
				.ForMember(m => m.Email, opt => opt.MapFrom(src => src.User.Email))
				.ForMember(m => m.Enabled, opt => opt.MapFrom(src => src.User.Enabled))
				.ForMember(m => m.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
				.ForMember(m => m.LastName, opt => opt.MapFrom(src => src.User.LastName))
				.ForMember(m => m.Phone, opt => opt.MapFrom(src => src.User.Phone))
				.ForMember(m => m.CreatedDate, opt => opt.MapFrom(src => src.User.CreatedDate))
				.ForMember(m => m.PaymentMethodSetup, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Brand.PaymentAccountId)))
				.ForMember(m => m.PaymentAccountId, opt => opt.MapFrom(src => src.Brand.PaymentAccountId));

			CreateMap<Area, AreaModel>();

			CreateMap<AdvocateBrand, AdvocateBrandModel>()
				.IncludeMembers(x => x.Brand)
				.ForMember(x => x.IsQuizRequired, x => x.MapFrom(y => y.Brand.QuizId.HasValue));

			CreateMap<Question, QuestionModel>()
				.ForMember(m => m.QuestionId, opt => opt.MapFrom(src => src.Id))
				.ForMember(m => m.QuestionOptionCombos, opt => opt.MapFrom(src => src.QuestionOptionCombos
					.OrderBy(x => x.Order)
					.Select(x => new QuestionOptionComboModel()
					{
						Id = x.Id,
						ComboOptionTitle = x.ComboOptionTitle,
						ComboOptionType = x.ComboOptionType.ToString(),
						Enabled = x.Enabled,
						OptionsPerRow = x.OptionsPerRow,
						ComboOptions = x.ComboOptions.Select(x => new QuestionOptionModel()
						{
							QuestionOptionId = x.Id,
							Order = x.Order,
							Text = x.Text,
							SubText = x.SubText,
							Enabled = x.Enabled,
							Optional = x.Optional
						}).OrderBy(x => x.Order).ToList()
					}).ToList()))
				.ForMember(m => m.QuestionOptions, opt => opt.MapFrom(src => src.QuestionOptions
					.Where(x => x.QuestionOptionComboId == null)
					.Select(x => new QuestionOptionModel()
					{
						QuestionOptionId = x.Id,
						Order = x.Order,
						Text = x.Text,
						SubText = x.SubText,
						Enabled = x.Enabled,
						Optional = x.Optional,
						QuestionDependencies = x.QuestionDependencies.Select(x => x.QuestionId).ToList(),
						OptionDependencies = x.QuestionOptionDependencies.Select(x => x.QuestionOptionDependencyTargetId).ToList()
					}).OrderBy(x => x.Order).ToList()));

			CreateMap<QuestionType, QuestionTypeModel>();

			CreateMap<ApplicationAnswerModel, ApplicationAnswer>()
				.ForMember(m => m.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
				.ForMember(m => m.Answers, opt => opt.MapFrom(src => src.Answers))
				.ForAllOtherMembers(m => m.Ignore());

			CreateMap<AnswerModel, Answer>()
					.ForMember(m => m.QuestionOptionId, opt => opt.MapFrom(src => src.QuestionOptionId))
					.ForMember(m => m.StaticAnswer, opt => opt.MapFrom(src => src.StaticAnswer))
					.ForMember(m => m.QuestionOptionComboId, opt => opt.MapFrom(src => src.QuestionOptionComboId))
					.ForAllOtherMembers(m => m.Ignore());

			CreateMap<ProfileBrand, ProfileBrandModel>()
					.ForMember(m => m.BrandId, opt => opt.MapFrom(src => src.Id))
					.ForMember(m => m.BrandName, opt => opt.MapFrom(src => src.BrandName))
					.ForAllOtherMembers(m => m.Ignore());

			CreateMap<Advocate, AdvocateSearchModel>()
					.ForMember(dst => dst.OnboardingItemsCompleted, opt => opt.MapFrom(src =>
						(src.PaymentEmailVerified ? 1 : 0) +
						(src.PracticeComplete ? 1 : 0) +
						(src.VideoWatched ? 1 : 0) +
						(src.IdentityVerificationStatus == IdentityVerificationStatus.Completed ? 1 : 0)
					))
					.ForMember(dst => dst.FullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : ""))
					.ForMember(dst => dst.Brands, opt => opt.MapFrom(src =>
						src.Brands.Select(br =>
							new AdvocateBrandSearchModel
							{
								QuizId = br.Brand.QuizId,
								QuizStatus = br.Brand.QuizId.HasValue ? GetQuizStatus(br.Brand, br.BrandId, br.AdvocateId) : 0,
								ContractStatus = br.ContractAccepted ? 1 : 0,
								InductionStatus = GetInductionStatus(br.Brand, br.BrandId, br.AdvocateId),
								BrandId = br.BrandId,
								BrandName = br.Brand.Name,
								IsPractice = br.Brand.IsPractice,
								Authorized = br.Authorized,
								Enabled = br.Enabled,
								Blocked = src.Status == AdvocateStatus.Blocked,
								Csat = br.Csat,
								BrandBlocked = br.Blocked
							})))
					.ForMember(dst => dst.BrandStatus, opt => opt.Ignore())
					.ForMember(dst => dst.QuizAttempts, opt => opt.MapFrom(src =>
						src.QuizAttempts.Where(aa => !aa.Result)
							.GroupBy(aa => aa.QuizId)
							.Select(aa => new AdvocateQuizSearchModel
							{
								QuizId = aa.Key,
								FailedAttempt = aa.Count()
							})));


			CreateMap<AdvocateSearchModel, AdvocateSearchResponseModel>();

			CreateMap<Tag, TagModel>()
				.ForMember(src => src.ToolTip, dst => dst.MapFrom(x => x.Description))
				.AfterMap((src, dst) => dst.SubTags = dst.SubTags.Any() ? dst.SubTags : null);

			CreateMap<CreateTagModel, CreateTagCommand>()
				.IgnoreAllPropertiesWithAnInaccessibleSetter();

			CreateMap<CreateTagCommand, Tag>()
				.ForMember(dst => dst.Id, opt => opt.Ignore())
				.ForMember(dst => dst.Enabled, opt => opt.Ignore())
				.ForMember(dst => dst.ParentTag, opt => opt.Ignore())
				.ForMember(dst => dst.ParentTagId, opt => opt.Ignore())
				.ForMember(dst => dst.Description, opt => opt.Ignore())
				.ForMember(dst => dst.L1PostClosureDisable, opt => opt.Ignore())
				.ForMember(dst => dst.L2PostClosureDisable, opt => opt.Ignore());

			CreateMap<ApplicationAnswer, AdvocateApplicationProfileModel>();

			CreateMap<Answer, AdvocateApplicationProfileAnswerModel>();
		}

		private int GetQuizStatus(Brand brand, Guid brandId, Guid advocateId)
		{
			return brand.InductionSections.Where(s => s.BrandId == brandId).OrEmpty().Any() ?
					brand.InductionSections.Where(s => s.BrandId == brandId).LastOrDefault().SectionItems.OrEmpty().Any() ?
					brand.InductionSections.Where(s => s.BrandId == brandId).LastOrDefault().SectionItems.LastOrDefault().AdvocateSectionItems.Where(aci => aci.AdvocateId == advocateId).OrEmpty().Any() ?
					1 : 0 : 0 : 0;
		}

		private int GetInductionStatus(Brand brand, Guid brandId, Guid advocateId)
		{
			return brand.InductionSections.Where(s => s.BrandId == brandId).OrEmpty().Any() ?
					brand.InductionSections.Where(s => s.BrandId == brandId).FirstOrDefault().SectionItems.OrEmpty().Any() ?
					brand.InductionSections.Where(s => s.BrandId == brandId).FirstOrDefault().SectionItems.FirstOrDefault().AdvocateSectionItems.Where(aci => aci.AdvocateId == advocateId).OrEmpty().Any() ?
					1 : 0 : 0 : 0;
		}
	}
}