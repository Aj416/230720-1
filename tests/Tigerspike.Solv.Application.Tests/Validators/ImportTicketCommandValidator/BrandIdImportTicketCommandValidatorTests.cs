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
	public class BrandIdImportTicketCommandValidatorTests : ImportTicketCommandValidatorBase
	{

		[Fact]
		public async Task WhenBrandIdIsDefaultThenImportTicketCommandShouldBeInvalid()
		{
			var brandId = Guid.Empty;
			var cmd = ImportTicketCommandBuilder.Build(brandId: brandId);

			Mocker.GetMock<IBrandRepository>()
				.Setup(x => x.ExistsAsync(
						It.IsAny<Expression<Func<Brand, bool>>>()
					)
				)
				.ReturnsAsync(false);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.Contains(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.BrandId) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.BrandIdNotExists)
			);
		}

		[Fact]
		public async Task WhenBrandIdDoesNotExistsThenImportTicketCommandShouldBeInvalid()
		{
			var brandId = Guid.NewGuid();
			var cmd = ImportTicketCommandBuilder.Build(brandId: brandId);

			Mocker.GetMock<IBrandRepository>()
				.Setup(x => x.ExistsAsync(
						It.IsAny<Expression<Func<Brand, bool>>>()
					)
				)
				.ReturnsAsync(false);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.Contains(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.BrandId) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.BrandIdNotExists)
			);
		}

		[Fact]
		public async Task WhenBrandIdExistsThenImportTicketCommandShouldBeValid()
		{
			var brandId = Guid.NewGuid();
			var cmd = ImportTicketCommandBuilder.Build(brandId: brandId);

			Mocker.GetMock<IBrandRepository>()
				.Setup(x => x.ExistsAsync(
						It.IsAny<Expression<Func<Brand, bool>>>()
					)
				)
				.ReturnsAsync(true);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.DoesNotContain(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.BrandId) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.BrandIdNotExists)
			);
		}
	}
}