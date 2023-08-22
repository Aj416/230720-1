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
	public class ImportIdImportTicketCommandValidatorTests : ImportTicketCommandValidatorBase
	{

		[Fact]
		public async Task WhenImportIdIsDefaultThenImportTicketCommandShouldBeInvalid()
		{
			var importId = Guid.Empty;
			var cmd = ImportTicketCommandBuilder.Build(importId: importId);

			Mocker.GetMock<ITicketImportRepository>()
				.Setup(x => x.ExistsAsync(
						It.IsAny<Expression<Func<TicketImport, bool>>>()
					)
				)
				.ReturnsAsync(false);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.Contains(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.ImportId) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.ImportIdNotExists)
			);
		}

		[Fact]
		public async Task WhenImportIdDoesNotExistsThenImportTicketCommandShouldBeInvalid()
		{
			var importId = Guid.NewGuid();
			var cmd = ImportTicketCommandBuilder.Build(importId: importId);

			Mocker.GetMock<ITicketImportRepository>()
				.Setup(x => x.ExistsAsync(
						It.IsAny<Expression<Func<TicketImport, bool>>>()
					)
				)
				.ReturnsAsync(false);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.Contains(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.ImportId) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.ImportIdNotExists)
			);
		}

		[Fact]
		public async Task WhenImportIdExistsThenImportTicketCommandShouldBeValid()
		{
			var importId = Guid.NewGuid();
			var cmd = ImportTicketCommandBuilder.Build(importId: importId);

			Mocker.GetMock<ITicketImportRepository>()
				.Setup(x => x.ExistsAsync(
						It.IsAny<Expression<Func<TicketImport, bool>>>()
					)
				)
				.ReturnsAsync(true);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.DoesNotContain(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.ImportId) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.ImportIdNotExists)
			);
		}
	}
}