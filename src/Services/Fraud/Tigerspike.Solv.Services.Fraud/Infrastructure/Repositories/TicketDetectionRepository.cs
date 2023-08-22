using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.Fraud.Infrastructure.Repositories
{
	/// <inheritdoc /> 
	public class TicketDetectionRepository : ITicketDetectionRepository
	{
		private readonly IPocoDynamo _db;

		/// <summary>
		/// TicketDetectionRepository Parameterised constructor.
		/// </summary>
		public TicketDetectionRepository(IPocoDynamo db) => _db = db;

		/// <inheritdoc />
		public TicketDetection AddOrUpdateDetection(TicketDetection ticketDetection) => _db.PutItem(ticketDetection);

		/// <inheritdoc />
		public void DeleteDetection(string detectionId) =>
			_db.DeleteItem<TicketDetection>(new DynamoId { Hash = detectionId });

		/// <inheritdoc />
		public List<TicketDetection> GetList(Guid ticketId)
		{
			return _db
					.FromQuery<TicketDetection>()
					.KeyCondition(m => m.TicketId == ticketId.ToString())
					.Exec().ToList();
		}

		/// <inheritdoc />
		public TicketDetection GetTicketDetection(string id) => _db
		.FromQuery<TicketDetection>().KeyCondition(m => m.TicketId == id)
		.Exec().FirstOrDefault();
	}
}