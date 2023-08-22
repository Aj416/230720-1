using System.Linq;
using AutoMapper;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Application.AutoMapper
{
	public class QuizProfile : Profile
	{
		public QuizProfile()
		{
			CreateMap<Quiz, QuizModel>()
				.ForMember(x => x.Questions, src => src.MapFrom(x => x.Questions.OrderBy(o => o.Order)));
			CreateMap<QuizQuestion, QuizQuestionModel>()
				.ForMember(x => x.Options, src => src.MapFrom(x => x.Options.OrderBy(o => o.Order)));
	
		}
	}
}