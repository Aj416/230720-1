using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Chat.Domain;

namespace Tigerspike.Solv.Chat.Infrastructure.Repositories
{
	public class MessageWhitelistRepository : IMessageWhitelistRepository
	{
		private readonly IPocoDynamo _db;

		public MessageWhitelistRepository(IPocoDynamo db) => _db = db;

		/// <inheritdoc />
		public List<MessageWhitelist> GetList(Guid brandId)
		{
			return _db
				.FromQuery<MessageWhitelist>().KeyCondition(m => m.BrandId == brandId.ToString())
				.Exec().ToList();
		}

		/// <inheritdoc />
		public MessageWhitelist AddOrUpdateMessage(MessageWhitelist message) => _db.PutItem(message);

		/// <inheritdoc />
		public void DeleteMessage(Guid brandId, string messageId) =>
			_db.DeleteItem<MessageWhitelist>(new DynamoId(brandId.ToString(), messageId));
	}
}