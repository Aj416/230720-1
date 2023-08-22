using FizzWare.NBuilder;
using System;
using System.Linq;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.TicketRepositoryTests
{
	public class GetAdvocatesToInvoiceTests : BaseRepositoryTest<TicketRepository>
	{

		[Fact]
		public async void ShouldReturnEmptyListWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetAdvocatesToInvoice(DateTime.MinValue, DateTime.MaxValue);

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public async void ShouldReturnSingleItemWhenAdvocateHasManyEligbleTickets()
		{
			// Arrange
			var advocateId = Guid.NewGuid();
			var fromDate = new DateTime(2020, 1, 1);
			var toDate = new DateTime(2020, 1, 14);

			SystemUnderTest.Insert(new [] { 
				GetTicket(Guid.NewGuid(), advocateId, 1.00m, new DateTime(2020, 1, 1)),
				GetTicket(Guid.NewGuid(), advocateId, 2.00m, new DateTime(2020, 1, 2)),
				GetTicket(Guid.NewGuid(), advocateId, 3.00m, new DateTime(2020, 1, 3)),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAdvocatesToInvoice(fromDate, toDate);

			// Assert
			Assert.Equal(advocateId, result.Single());
		}

		private Ticket GetTicket(Guid brandId, Guid advocateId, decimal price, DateTime? closedDate = null)
		{
			return Builder<Ticket>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.BrandId, brandId)
				.With(x => x.AdvocateId, advocateId)
				.With(x => x.AdvocateInvoiceId, null)
				.With(x => x.Status, closedDate != null ? TicketStatusEnum.Closed : TicketStatusEnum.New)
				.With(x => x.ClosedDate, closedDate)
				.With(x => x.Price, (price * 0.7m))
                .With(x => x.Fee, (price * 0.3m))
                .Build();
		}
	}
}