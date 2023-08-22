using System.Linq;
using AutoMapper;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Application.AutoMapper
{
	public class AdvocateApplicationProfile : Profile
	{
		public AdvocateApplicationProfile()
		{
			CreateMap<AdvocateApplication, AdvocateApplicationModel>()
				.ForMember(a => a.Phone, opt => opt.MapFrom(src => src.Phone))
				.ForMember(a => a.Questionnaire, opt => opt.MapFrom(src => src.CompletedEmailSent))
				.ForMember(a => a.IsAdult, opt => opt.Ignore())
				.ForMember(a => a.MarketingCheckbox, opt => opt.Ignore())
				.ForMember(a => a.DataPolicyCheckbox, opt => opt.Ignore())
				.ForMember(a => a.Password, opt => opt.Ignore());			
				

			CreateMap<AdvocateApplication, AdvocateApplicationSearchModel>()
				.ForMember(x => x.ApplicationStatus, opt => opt.MapFrom(x => (int)x.ApplicationStatus))
				.ForMember(x => x.Language, opt => opt.MapFrom(m => m.ApplicationAnswers
					.SelectMany(x => x.Answers
						.Where(s => s.QuestionOption.BusinessValue == QuestionBusinessValue.Language)
						.Select(s => s.QuestionOption.Text)
					)
					.OrderBy(x => x.ToLower())
				))
				.ForMember(x => x.Skills, opt => opt.MapFrom(m => m.ApplicationAnswers
					.SelectMany(x => x.Answers
						.Where(s => s.QuestionOption.BusinessValue == QuestionBusinessValue.Skills)
						.Select(s => new ProfileSkillModel {
							Name = s.QuestionOption.Text,
							RawLevel = s.StaticAnswer
						}))
					.OrderBy(x => x.Name)

				))
				.ForMember(a => a.Questionnaire, opt => opt.MapFrom(src => src.CompletedEmailSent));

			CreateMap<AdvocateApplicationSearchModel, AdvocateApplicationSearchResponseModel>()
				.ForMember(x => x.Skills, opt => opt.MapFrom(x => x.Skills.Select(s => s.Display)));
		}
	}
}