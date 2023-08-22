using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateCommandHandlerTests
{
	public class AgreeToContractAdvocateCommandHandlerTests : BaseClass
	{
		

		[Fact]
		public async Task ShouldSetContractAcceptedPropertyWhenHandledProperly()
		{
			// Arrange
			var advocateBrand = Builder<AdvocateBrand>
				.CreateNew()
				.With(x => x.AdvocateId, Guid.NewGuid())
				.With(x => x.BrandId, Guid.NewGuid())
				.With(x => x.ContractAccepted, false)
				.Build();
			var advocate = Builder<Advocate>
				.CreateNew()
				.With(x => x.Id, advocateBrand.AdvocateId)
				.Build();

			MockAdvocateBrandRepository
				.Setup(x => x.FindAsync(advocateBrand.AdvocateId, advocateBrand.BrandId))
				.ReturnsAsync(advocateBrand);

			MockAdvocateRepository
				.Setup(x => x.FindAsync(advocate.Id))
				.ReturnsAsync(advocate);

			var cmd = new AgreeToContractCommand(advocateBrand.AdvocateId, advocateBrand.BrandId);

			// Act
			await AdvocateCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.True(advocateBrand.ContractAccepted);
		}

		[Fact]
		public async Task ShouldNotChangeBrandNotificationsWhenInductionIsNotPassed()
		{
			// Arrange
			var advocateBrand = Builder<AdvocateBrand>
				.CreateNew()
				.With(x => x.AdvocateId, Guid.NewGuid())
				.With(x => x.BrandId, Guid.NewGuid())
				.With(x => x.ContractAccepted, false)
				.With(x => x.Inducted, false)
				.Build();
			var advocate = Builder<Advocate>
				.CreateNew()
				.With(x => x.Id, advocateBrand.AdvocateId)
				.With(x => x.ShowBrandNotification, true)
				.Build();

			MockAdvocateBrandRepository
				.Setup(x => x.FindAsync(advocateBrand.AdvocateId, advocateBrand.BrandId))
				.ReturnsAsync(advocateBrand);

			MockAdvocateRepository
				.Setup(x => x.FindAsync(advocate.Id))
				.ReturnsAsync(advocate);

			var cmd = new AgreeToContractCommand(advocateBrand.AdvocateId, advocateBrand.BrandId);

			// Act
			await AdvocateCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.True(advocate.ShowBrandNotification);
		}

		[Fact]
		public async Task ShouldDismissBrandNotificationsWhenInductionIsPassed()
		{
			// Arrange
			var advocateBrand = Builder<AdvocateBrand>
				.CreateNew()
				.With(x => x.AdvocateId, Guid.NewGuid())
				.With(x => x.BrandId, Guid.NewGuid())
				.With(x => x.ContractAccepted, false)
				.With(x => x.Inducted, true)
				.Build();
			var advocate = Builder<Advocate>
				.CreateNew()
				.With(x => x.Id, advocateBrand.AdvocateId)
				.With(x => x.ShowBrandNotification, true)
				.Build();

			MockBrandRepository
				.Setup(x =>x.GetSingleOrDefaultAsync(
					It.IsAny<Expression<Func<Brand, bool>>>(),
					It.IsAny<Expression<Func<Brand, bool>>>(),
					It.IsAny<Func<IQueryable<Brand>, IOrderedQueryable<Brand>>>(),
					It.IsAny<Func<IQueryable<Brand>, IIncludableQueryable<Brand, object>>>(),
					It.IsAny<bool>()))
				.ReturnsAsync(true);

			MockAdvocateBrandRepository
				.Setup(x => x.FindAsync(advocateBrand.AdvocateId, advocateBrand.BrandId))
				.ReturnsAsync(advocateBrand);

			MockAdvocateRepository
				.Setup(x => x.FindAsync(advocate.Id))
				.ReturnsAsync(advocate);

			var cmd = new AgreeToContractCommand(advocateBrand.AdvocateId, advocateBrand.BrandId);

			// Act
			await AdvocateCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.False(advocate.ShowBrandNotification);
		}
	}
}