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
	public class ReferenceIdImportTicketCommandValidatorTests : ImportTicketCommandValidatorBase
	{

		[Fact]
		public async Task WhenReferenceIdIsDefaultThenImportTicketCommandShouldBeInvalid()
		{
			string referenceId = null;
			var cmd = ImportTicketCommandBuilder.Build(referenceId: referenceId);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.Contains(result.Errors, x => 
				x.PropertyName == nameof(ImportTicketCommand.ReferenceId) && 
				x.ErrorCode == nameof(FluentValidation.Validators.NotEmptyValidator)
			);			
		}

		[Fact]
		public async Task WhenReferenceIdIsNotUniqueThenImportTicketCommandShouldBeInvalid()
		{
			string referenceId = "123";
			var brandId = Guid.NewGuid();
			var cmd = ImportTicketCommandBuilder.Build(referenceId: referenceId, brandId: brandId);

			Mocker.GetMock<ITicketRepository>()
				.Setup(x => x.CountAsync(
						It.IsAny<Expression<Func<Ticket, bool>>>()
					)
				)
				.ReturnsAsync(1);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.Contains(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.ReferenceId) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.ReferenceIdNotUnique)
			);
		}

		[Fact]
		public async Task WhenReferenceIdIsUniqueThenImportTicketCommandShouldBeValid()
		{
			string referenceId = "123";
			var brandId = Guid.NewGuid();
			var cmd = ImportTicketCommandBuilder.Build(referenceId: referenceId, brandId: brandId);

			Mocker.GetMock<ITicketRepository>()
				.Setup(x => x.CountAsync(
						It.IsAny<Expression<Func<Ticket, bool>>>()
					)
				)
				.ReturnsAsync(0);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.DoesNotContain(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.ReferenceId) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.ReferenceIdNotUnique)
			);
		}
	}
}