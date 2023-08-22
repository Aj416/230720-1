using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Enum;
using Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.Fraud.Infrastructure.Repositories
{
	/// <inheritdoc />
	public class TicketRepository : ITicketRepository
	{
		private readonly IPocoDynamo _db;

		/// <summary>
		/// TicketRepository Parameterised constructor.
		/// </summary>
		public TicketRepository(IPocoDynamo db) => _db = db;

		/// <inheritdoc />
		public Ticket AddOrUpdateTicket(Ticket ticket) => _db.PutItem(ticket);

		/// <inheritdoc />
		public void DeleteTicket(Guid ticketId) =>
			_db.DeleteItem<Ticket>(new DynamoId { Hash = ticketId.ToString() });

		/// <inheritdoc />
		public Ticket GetTicket(string id) => _db
				.FromQuery<Ticket>().KeyCondition(m => m.TicketId == id)
				.Exec().FirstOrDefault();

		/// <inheritdoc />
		public async Task<IEnumerable<Guid>> GetFraudTicketIds() =>
			await GetFraudIds();

		/// <inheritdoc />
		public void BulkUpdateTicket(IEnumerable<Ticket> tickets) => _db.PutItems(tickets);

		/// <inheritdoc />
		public Task<List<Guid>> GetFraudIds()
		{
			var list = new List<Guid>();

			var fraudConfirmedresult = _db.FromQueryIndex<TicketFraudStatusGlobalIndex>(x =>
				x.FraudStatus.Equals(FraudStatus.FraudConfirmed.ToString(), StringComparison.InvariantCultureIgnoreCase))
				.Exec();

			list.AddRange(fraudConfirmedresult.Select(x => Guid.Parse(x.TicketId)).ToList());

			var fraudSuspectedresult = _db.FromQueryIndex<TicketFraudStatusGlobalIndex>(x =>
				x.FraudStatus.Equals(FraudStatus.FraudSuspected.ToString(), StringComparison.InvariantCultureIgnoreCase))
				.Exec();

			list.AddRange(fraudSuspectedresult.Select(x => Guid.Parse(x.TicketId)).ToList());

			var notFraudulentresult = _db.FromQueryIndex<TicketFraudStatusGlobalIndex>(x =>
				x.FraudStatus.Equals(FraudStatus.NotFraudulent.ToString(), StringComparison.InvariantCultureIgnoreCase))
				.Exec();

			list.AddRange(notFraudulentresult.Select(x => Guid.Parse(x.TicketId)).ToList());

			return Task.FromResult(list);
		}
		
		/// <inheritdoc />
		public Task<List<Ticket>> GetCustomerTicket(string customerId)
		{
			var tickets = _db.FromQueryIndex<Ticket>(x => x.CustomerId.Equals(customerId)).Exec().ToList();

			return Task.FromResult(tickets);
		}
	}
}