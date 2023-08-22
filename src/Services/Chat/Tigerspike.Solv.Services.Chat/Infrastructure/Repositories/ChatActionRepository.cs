using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Chat.Domain;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Services.Chat.Domain;
using Action = Tigerspike.Solv.Chat.Domain.Action;

namespace Tigerspike.Solv.Services.Chat.Infrastructure.Repositories
{
	public class ChatActionRepository : IChatActionRepository
	{
		private readonly IPocoDynamo _db;

		public ChatActionRepository(IPocoDynamo db) => _db = db;

		public async Task<Action> GetById(Guid chatActionId)
		{
			var action = _db
				.FromQuery<Action>()
				.KeyCondition($"HashKey = :hashKey  and RangeKey = :rangeKey",
					new Dictionary<string, string>()
					{
						{ "hashKey", $"{nameof(Action)}#{chatActionId}" },
						{ "rangeKey", $"{nameof(Action)}#{chatActionId}" }
					})
				.Exec().SingleOrDefault();

			action.Options = await _db
				.FromQuery<ActionOption>()
				.KeyCondition($"HashKey = :hashKey  and begins_with(RangeKey, :rangeKey)",
					new Dictionary<string, string>()
					{
						{ "hashKey", $"{nameof(Action)}#{chatActionId}" }, { "rangeKey", $"{nameof(ActionOption)}" }
					})
				.ExecAsync();

			return action;
		}
	}
}