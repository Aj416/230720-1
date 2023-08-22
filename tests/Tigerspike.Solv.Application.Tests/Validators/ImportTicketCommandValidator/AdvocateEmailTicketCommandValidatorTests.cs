using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Validators
{
	public class AdvocateEmailTicketCommandValidatorTests : ImportTicketCommandValidatorBase
	{

		[Fact]
		public async Task WhenAdvocateEmailIsEmptyAndAssignedDateIsEmptyThenImportTicketCommandShouldBeValid()
		{
			var cmd = ImportTicketCommandBuilder.Build();

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.DoesNotContain(result.Errors, x => x.PropertyName == nameof(ImportTicketCommand.AdvocateEmail));
			Assert.DoesNotContain(result.Errors, x => x.PropertyName == nameof(ImportTicketCommand.AssignedDate));
		}

		[Fact]
		public async Task WhenAdvocateEmailIsEmptyAndAssignedDateIsProvidedThenImportTicketCommandShouldBeInvalid()
		{
			var cmd = ImportTicketCommandBuilder.Build(advocateEmail: null, assignedDate: new DateTime(2020, 1, 1));

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.Contains(result.Errors, x => x.PropertyName == nameof(ImportTicketCommand.AdvocateEmail));
		}

		[Fact]
		public async Task WhenAdvocateEmailIsNotRegisteredThenImportTicketCommandShouldBeInvalid()
		{
			var advocateEmail = "test@test.com";
			var brandId = Guid.NewGuid();
			var cmd = ImportTicketCommandBuilder.Build(advocateEmail: advocateEmail, assignedDate: new DateTime(2020, 1, 1), brandId: brandId);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.Contains(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.AdvocateEmail) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.AdvocateNotExists)
			);
		}

		[Fact]
		public async Task WhenAdvocateEmailIsNotAuthorisedThenImportTicketCommandShouldBeInvalid()
		{
			var advocateEmail = "test@test.com";
			var brandId = Guid.NewGuid();
			var cmd = ImportTicketCommandBuilder.Build(advocateEmail: advocateEmail, assignedDate: new DateTime(2020, 1, 1), brandId: brandId);

			var adovcateBrand = Builder<AdvocateBrand>
				.CreateNew()
				.With(x => x.BrandId, brandId)
				.With(x => x.Enabled, false)
				.Build();

			var advocate = Builder<Advocate>
				.CreateNew()
				.With(x => x.Brands, new[] { adovcateBrand }.ToList())
				.Build();

			Mocker.GetMock<IAdvocateRepository>()
				.Setup(x => x.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<Advocate, bool>>>(),
					It.IsAny<Func<IQueryable<Advocate>, IOrderedQueryable<Advocate>>>(),
					It.IsAny<Func<IQueryable<Advocate>, IIncludableQueryable<Advocate, object>>>(),
					It.IsAny<bool>(), It.IsAny<bool>()
					)
				)
				.ReturnsAsync(advocate);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.Contains(result.Errors, x =>
				x.PropertyName == nameof(ImportTicketCommand.AdvocateEmail) &&
				x.ErrorCode == nameof(ImportTicketCommandValidator.AdvocateNotAuthorised)
			);
		}

		[Fact]
		public async Task WhenAdvocateEmailIsAuthorisedThenImportTicketCommandShouldBeValid()
		{
			var advocateEmail = "test@test.com";
			var brandId = Guid.NewGuid();
			var cmd = ImportTicketCommandBuilder.Build(advocateEmail: advocateEmail, assignedDate: new DateTime(2020, 1, 1), brandId: brandId);

			var adovcateBrand = Builder<AdvocateBrand>
				.CreateNew()
				.With(x => x.BrandId, brandId)
				.With(x => x.Authorized, true)
				.Build();

			var advocate = Builder<Advocate>
				.CreateNew()
				.With(x => x.Brands, new[] { adovcateBrand }.ToList())
				.Build();

			Mocker.GetMock<IAdvocateRepository>()
				.Setup(x => x.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<Advocate, bool>>>(),
					It.IsAny<Func<IQueryable<Advocate>, IOrderedQueryable<Advocate>>>(),
					It.IsAny<Func<IQueryable<Advocate>, IIncludableQueryable<Advocate, object>>>(),
					It.IsAny<bool>(), It.IsAny<bool>()
					)
				)
				.ReturnsAsync(advocate);

			var result = await SystemUnderTest.ValidateAsync(cmd);
			Assert.DoesNotContain(result.Errors, x => x.PropertyName == nameof(ImportTicketCommand.AdvocateEmail));
		}		
	}
}