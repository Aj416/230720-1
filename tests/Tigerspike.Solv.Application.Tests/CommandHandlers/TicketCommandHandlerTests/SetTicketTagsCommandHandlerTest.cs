using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Tigerspike.Solv.Application.CommandHandlers;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Tests.CommandsHandlers;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.DTOs;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Xunit;


namespace Tigerspike.Solv.Application.Tests.CommandHandlers.TicketCommandHandlerTests
{
	public class SetTicketTagsCommandHandlerTest : BaseCommandHandlerTests<TicketCommandHandler>
	{
		public SetTicketTagsCommandHandlerTest()
		{
			Mocker.GetMock<IOptions<EmailTemplatesOptions>>()
				.Setup(s => s.Value)
				.Returns(new EmailTemplatesOptions
				{
					AdvocateExportEmailSubject = "subject",
					EmailLogoLocation = "location",
					AdvocateExportEmailAttachmentContentType = "type",
					AdvocateExportEmailAttachmentFileName = "filename"
				});
		}

		[Fact]
		public async Task NoTagHasAnExtraAction()
		{
			// Arrange
			var ticketId = Guid.NewGuid();
			var brandId = Guid.NewGuid();

			var tagId = Guid.NewGuid();
			var tagId1 = Guid.NewGuid();

			Advocate supersolver = new Advocate(new User(Guid.NewGuid(), "John", "Doe"), "US", "source", true, true, true);

			Guid[] tagIds = { tagId, tagId1 };

			var cmd = new SetTicketTagsCommand(ticketId, tagIds, TicketLevel.Regular);

			var tagList = new List<Tag>(){
				Builder<Tag>.CreateNew()
				.With(x => x.Id ,tagId)
				.With(x => x.Name, "hardware")
				.With(x => x.Action, null)
				.With(x => x.BrandId, brandId)
                .With(x => x.Level, 1)
                .With(x => x.ParentTagId, null)
				.Build(),
				Builder<Tag>.CreateNew()
				.With(x => x.Id ,tagId1)
				.With(x => x.Name, "warranty")
				.With(x => x.Action, null)
				.With(x => x.BrandId, brandId)
                .With(x => x.Level, 1)
                .With(x => x.ParentTagId, null)
				.Build(),
				Builder<Tag>.CreateNew()
				.With(x => x.Id ,Guid.NewGuid())
				.With(x => x.Name, "software")
				.With(x => x.Action, TicketFlowAction.Escalate)
				.With(x => x.BrandId, brandId)
                .With(x => x.Level, 1)
                .With(x => x.ParentTagId, null)
				.Build()
			};

			var mockBrand = Builder<Brand>
				.CreateNew()
				.With(x => x.Id, brandId)
				.With(x => x.Name, "test brand")
				.With(x => x.TagsEnabled, true)
				.With(x => x.Tags, tagList)
				.Build();

			var mockTicket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Id, ticketId)
				.With(x => x.Brand, mockBrand)
				.With(x => x.BrandId, brandId)
				.With(x => x.Tags, new List<TicketTag>())
				.With(x => x.Advocate, supersolver)
				.Build();

			var mockTicketRepository = Mocker.GetMock<ITicketRepository>();
			mockTicketRepository.Setup(x => x.FindAsync(It.IsAny<Guid>()))
						.Returns(new ValueTask<Ticket>(mockTicket));
			mockTicketRepository.Setup(x => x.GetTicketWithTaggingInfo(It.IsAny<Expression<Func<Ticket, bool>>>()))
						.ReturnsAsync(mockTicket);

			var mockBrandRepository = Mocker.GetMock<IBrandRepository>();
			mockBrandRepository.Setup(x => x.FindAsync(It.IsAny<Guid>()))
						.Returns(new ValueTask<Brand>(mockBrand));
			mockBrandRepository.Setup(x => x.GetTags(brandId, true, TicketLevel.Regular))
						.ReturnsAsync(tagList);

			Mocker.GetMock<ITicketTagRepository>().SetReturnsDefault<Task<IList<TicketTag>>>(Task.FromResult((IList<TicketTag>)new List<TicketTag>()));			
			Mocker.GetMock<IUnitOfWork>().Setup(uow => uow.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));

			//Act
			Assert.Equal(Unit.Value, await SystemUnderTest.Handle(cmd, CancellationToken.None));
			Mocker.GetMock<IMediatorHandler>().Verify(x => x.RaiseEvent(It.IsAny<TicketTagsChangedEvent>()), Times.Once);
		}

		[Fact]
		public async Task OneTagHasAnExtraAction()
		{
			// Arrange
			var ticketId = Guid.NewGuid();
			var brandId = Guid.NewGuid();

			var tagId = Guid.NewGuid();
			var tagId1 = Guid.NewGuid();

			Advocate supersolver = new Advocate(new User(Guid.NewGuid(), "John", "Doe"), "US", "source", true, true, false);
            
			Guid[] tagIds = { tagId, tagId1 };

			var cmd = new SetTicketTagsCommand(ticketId, tagIds, TicketLevel.Regular);

			var tagList = new List<Tag>(){
				Builder<Tag>.CreateNew()
				.With(x => x.Id ,tagId)
				.With(x => x.Name, "hardware")
				.With(x => x.Action, null)
				.With(x => x.BrandId, brandId)
				.Build(),
				Builder<Tag>.CreateNew()
				.With(x => x.Id ,tagId1)
				.With(x => x.Name, "warranty")
				.With(x => x.Action, TicketFlowAction.Escalate)
				.With(x => x.BrandId, brandId)
				.Build(),
				Builder<Tag>.CreateNew()
				.With(x => x.Id ,Guid.NewGuid())
				.With(x => x.Name, "software")
				.With(x => x.Action, null)
				.With(x => x.BrandId, brandId)
				.Build()
			};

			var mockBrand = Builder<Brand>
				.CreateNew()
				.With(x => x.Id, brandId)
				.With(x => x.Name, "test brand")
				.With(x => x.TagsEnabled, true)
				.With(x => x.Tags, tagList)
				.Build();

			var mockTicket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Id, ticketId)
				.With(x => x.Brand, mockBrand)
				.With(x => x.BrandId, brandId)
				.With(x => x.Tags, new List<TicketTag>())
				.With(x => x.Advocate, supersolver)
				.Build();

			var mockTicketRepository = Mocker.GetMock<ITicketRepository>();
			mockTicketRepository.Setup(x => x.FindAsync(It.IsAny<Guid>()))
						.Returns(new ValueTask<Ticket>(mockTicket));
			mockTicketRepository.Setup(x => x.GetTicketWithTaggingInfo(It.IsAny<Expression<Func<Ticket, bool>>>()))
						.ReturnsAsync(mockTicket);

			var mockBrandRepository = Mocker.GetMock<IBrandRepository>();
			mockBrandRepository.Setup(x => x.FindAsync(It.IsAny<Guid>()))
						.Returns(new ValueTask<Brand>(mockBrand));
			mockBrandRepository.Setup(x => x.GetTags(brandId, true, TicketLevel.Regular))
						.ReturnsAsync(tagList);

			Mocker.GetMock<ITicketTagRepository>().SetReturnsDefault<Task<IList<TicketTag>>>(Task.FromResult((IList<TicketTag>)new List<TicketTag>()));	

			Mocker.GetMock<IUnitOfWork>().Setup(uow => uow.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));

			//Act
			Assert.Equal(Unit.Value, await SystemUnderTest.Handle(cmd, CancellationToken.None));
			Mocker.GetMock<IMediatorHandler>().Verify(x => x.RaiseEvent(It.IsAny<TicketTagsChangedEvent>()), Times.Once);
		}

		[Fact]
		public async Task MoreThenOneTagHasAnExtraAction()
		{
			// Arrange
			var ticketId = Guid.NewGuid();
			var brandId = Guid.NewGuid();

			var tagId = Guid.NewGuid();
			var tagId1 = Guid.NewGuid();

			Advocate supersolver = new Advocate(new User(Guid.NewGuid(), "John", "Doe"), "US", "source", true, true, false);

			Guid[] tagIds = { tagId, tagId1 };

			var cmd = new SetTicketTagsCommand(ticketId, tagIds, TicketLevel.Regular);

			var tagList = new List<Tag>(){
				Builder<Tag>.CreateNew()
				.With(x => x.Id ,tagId)
				.With(x => x.Name, "hardware")
				.With(x => x.Action, (TicketFlowAction)(0))
				.With(x => x.BrandId, brandId)
				.Build(),
				Builder<Tag>.CreateNew()
				.With(x => x.Id ,tagId1)
				.With(x => x.Name, "warranty")
				.With(x => x.Action, (TicketFlowAction)(1))
				.With(x => x.BrandId, brandId)
				.Build(),
				Builder<Tag>.CreateNew()
				.With(x => x.Id ,Guid.NewGuid())
				.With(x => x.Name, "software")
				.With(x => x.Action, null)
				.With(x => x.BrandId, brandId)
				.Build()
			};

			var mockBrand = Builder<Brand>
				.CreateNew()
				.With(x => x.Id, brandId)
				.With(x => x.Name, "test brand")
				.With(x => x.TagsEnabled, true)
				.With(x => x.Tags, tagList)
				.Build();

			var mockTicket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Id, ticketId)
				.With(x => x.Brand, mockBrand)
				.With(x => x.BrandId, brandId)
				.With(x => x.Tags, new List<TicketTag>())
				.With(x => x.Advocate, supersolver)
				.Build();

			var mockTicketRepository = Mocker.GetMock<ITicketRepository>();
			mockTicketRepository.Setup(x => x.FindAsync(It.IsAny<Guid>()))
						.Returns(new ValueTask<Ticket>(mockTicket));
			mockTicketRepository.Setup(x => x.GetTicketWithTaggingInfo(It.IsAny<Expression<Func<Ticket, bool>>>()))
						.ReturnsAsync(mockTicket);

			var mockBrandRepository = Mocker.GetMock<IBrandRepository>();
			mockBrandRepository.Setup(x => x.FindAsync(It.IsAny<Guid>()))
						.Returns(new ValueTask<Brand>(mockBrand));
			mockBrandRepository.Setup(x => x.GetTags(brandId, true, TicketLevel.Regular))
						.ReturnsAsync(tagList);

			Mocker.GetMock<ITicketTagRepository>().SetReturnsDefault<Task<IList<TicketTag>>>(Task.FromResult((IList<TicketTag>)new List<TicketTag>()));	
			Mocker.GetMock<IUnitOfWork>().Setup(uow => uow.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));

			//Act
			Assert.Equal(Unit.Value, await SystemUnderTest.Handle(cmd, CancellationToken.None));
			Mocker.GetMock<IMediatorHandler>().Verify(x => x.RaiseEvent(It.IsAny<TicketTagsChangedEvent>()), Times.Never);
		}
	}
}