using System;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateCommandHandlerTests
{
	public class EnableBrandAdvocateCommandHandlerTests : BaseClass
	{	

		[Fact]
		public async Task ShouldSetEnabledPropertyWhenHandledProperly()
		{
			// Arrange
			var advocateBrand = Builder<AdvocateBrand>
				.CreateNew()
				.With(x => x.AdvocateId, Guid.NewGuid())
				.With(x => x.BrandId, Guid.NewGuid())
				.With(x => x.Enabled, false)
				.Build();

			MockAdvocateBrandRepository
				.Setup(x => x.FindAsync(advocateBrand.AdvocateId, advocateBrand.BrandId))
				.ReturnsAsync(advocateBrand);

			var cmd = new EnableBrandCommand(advocateBrand.AdvocateId, advocateBrand.BrandId);

			// Act
			await AdvocateCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.True(advocateBrand.Enabled);
		}

		[Fact]
		public async Task ShouldLeaveEnabledPropertyWhenBrandWasAlreadyEnabled()
		{
			// Arrange
			var advocateBrand = Builder<AdvocateBrand>
				.CreateNew()
				.With(x => x.AdvocateId, Guid.NewGuid())
				.With(x => x.BrandId, Guid.NewGuid())
				.With(x => x.Enabled, true)
				.Build();

			MockAdvocateBrandRepository
				.Setup(x => x.FindAsync(advocateBrand.AdvocateId, advocateBrand.BrandId))
				.ReturnsAsync(advocateBrand);

			var cmd = new EnableBrandCommand(advocateBrand.AdvocateId, advocateBrand.BrandId);

			// Act
			await AdvocateCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.True(advocateBrand.Enabled);
		}

	}
}