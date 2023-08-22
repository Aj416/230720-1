using Moq;
using System;
using System.Collections.Generic;
using Tigerspike.Solv.Application.CommandHandlers;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Profile;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.ProfileCommandHandlerTests
{
	public class BaseClass
	{
		protected static readonly Guid Id = new Guid("14efbbe9-ce7b-4b67-b2a5-873189a68c3e");
		protected readonly Mock<IAdvocateApplicationRepository> MockAdvocateApplicationRepository;
		protected readonly Mock<IApplicationAnswerRepository> MockApplicationAnswerRepository;
		protected readonly Mock<IUnitOfWork> MockUnitOfWork;
		protected readonly Mock<IMediatorHandler> MockMediator;
		protected readonly Mock<IDomainNotificationHandler> MockNotificationHandler;

		protected readonly ProfileCommandHandler ProfileCommandHandler;

		protected BaseClass()
		{
			MockAdvocateApplicationRepository = new Mock<IAdvocateApplicationRepository>();
			MockApplicationAnswerRepository = new Mock<IApplicationAnswerRepository>();
			MockUnitOfWork = new Mock<IUnitOfWork>();
			MockMediator = new Mock<IMediatorHandler>();
			MockNotificationHandler = new Mock<IDomainNotificationHandler>();

			ProfileCommandHandler = new ProfileCommandHandler(
				MockAdvocateApplicationRepository.Object,
				MockApplicationAnswerRepository.Object,
				MockUnitOfWork.Object,
				MockMediator.Object,
				MockNotificationHandler.Object);
		}

		protected static ApplicationAnswer GetMockApplicationAnswer()
		{
			return new ApplicationAnswer
			{
				Id = Guid.NewGuid(),
				AdvocateApplicationId = Id,
				AdvocateApplication = new AdvocateApplication("Test", "User", "test@test.com", "+123 456 789", "NYC", "USA", "facebook", false, "102, Down Street", "Los Angeles", "10001"),
				QuestionId = Guid.NewGuid(),
				Answers = new List<Answer>
				{
					new Answer
					{
						Id = Guid.NewGuid(),
						ApplicationAnswerId = Id,
						QuestionOptionId = Guid.NewGuid(),
						StaticAnswer = "2"
					}
				}
			};
		}
	}
}