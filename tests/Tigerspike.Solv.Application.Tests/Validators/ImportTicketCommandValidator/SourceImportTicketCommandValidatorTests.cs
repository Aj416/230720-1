using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Validators
{
	public class SourceImportTicketCommandValidatorTests : ImportTicketCommandValidatorBase
	{

		[Fact]
		public async Task WhenSourceIsEmptyThenImportTicketCommandShouldBeValid()
		{
			string source = null;
			var cmd = ImportTicketCommandBuilder.Build(source: source);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.DoesNotContain(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.Source) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.SourceNotExists)
			);
		}

		[Fact]
		public async Task WhenSourceDoesNotExistsThenImportTicketCommandShouldBeInvalid()
		{
			string source = "test";
			var cmd = ImportTicketCommandBuilder.Build(source: source);

			Mocker.GetMock<ITicketSourceRepository>()
				.Setup(x => x.ExistsAsync(
						It.IsAny<Expression<Func<TicketSource, bool>>>()
					)
				)
				.ReturnsAsync(false);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.Contains(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.Source) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.SourceNotExists)
			);
		}

		[Fact]
		public async Task WhenSourceExistsThenImportTicketCommandShouldBeValid()
		{
			string source = "test";
			var cmd = ImportTicketCommandBuilder.Build(source: source);

			Mocker.GetMock<ITicketSourceRepository>()
				.Setup(x => x.ExistsAsync(
						It.IsAny<Expression<Func<TicketSource, bool>>>()
					)
				)
				.ReturnsAsync(true);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.DoesNotContain(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.Source) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.SourceNotExists)
			);
		}
	}
}