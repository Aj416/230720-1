using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateServiceTests
{
	public class BaseClass
	{
		protected readonly AdvocateService AdvocateService;
		protected readonly Mock<IAdvocateApplicationRepository> MockAdvocateApplicationRepository;
		protected readonly Mock<IAdvocateRepository> MockAdvocateRepository;
		protected readonly Mock<IAdvocateBrandRepository> MockAdvocateBrandRepository;
		protected readonly Mock<IMapper> Mapper;
		protected readonly Mock<IMediatorHandler> Mediator;
		protected readonly Mock<IOptions<EmailTemplatesOptions>> MockEmailTemplatesOptions;
		protected readonly Mock<IAuthenticationService> MockAuthenticationService;
		protected readonly Mock<IBrandRepository> MockBrandRepository;
		protected Mock<IQuestionRepository> MockQuestionRepository;
		protected Mock<IAreaRepository> MockAreaRepository;
		protected Mock<IApplicationAnswerRepository> MockApplicationAnswerRepository;


		protected BaseClass()
		{
			Mapper = new Mock<IMapper>();
			Mediator = new Mock<IMediatorHandler>();
			MockEmailTemplatesOptions = new Mock<IOptions<EmailTemplatesOptions>>();
			MockEmailTemplatesOptions.Setup(ser => ser.Value).Returns(new EmailTemplatesOptions());
			MockAdvocateRepository = new Mock<IAdvocateRepository>();
			MockAdvocateBrandRepository = new Mock<IAdvocateBrandRepository>();
			MockAdvocateApplicationRepository = new Mock<IAdvocateApplicationRepository>();
			MockAuthenticationService = new Mock<IAuthenticationService>();
			MockBrandRepository = new Mock<IBrandRepository>();
			MockQuestionRepository = new Mock<IQuestionRepository>();
			MockAreaRepository = new Mock<IAreaRepository>();
			MockApplicationAnswerRepository = new Mock<IApplicationAnswerRepository>();

			AdvocateService = new AdvocateService(
				MockBrandRepository.Object,
				MockAdvocateApplicationRepository.Object,
				MockAdvocateRepository.Object,
				MockAdvocateBrandRepository.Object,
				MockAuthenticationService.Object,
				Mapper.Object,
				MockAreaRepository.Object,
				MockQuestionRepository.Object,
				MockApplicationAnswerRepository.Object,
				Mediator.Object
				);
		}
	}
}