using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Enums;
using Tigerspike.Solv.Application.Models;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface IWebHookService
	{
		/// <summary>
		/// Publish occured event to all webhook subscribers
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="webHookEventType">The webhook event type</param>
		/// <param name="payload">Payload with key value pair</param>
		Task Publish(Guid brandId, WebHookEventTypes webHookEventType, IDictionary<string, object> payload);

		/// <summary>
		/// Registers webhook to a brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="userId">The user id</param>
		/// <param name="model">The create request</param>
		Task CreateSubscription(Guid brandId, Guid? userId, CreateWebHookModel model);

		/// <summary>
		/// Deletes webhook
		/// </summary>
		/// <param name="id">The id of webhook</param>
		/// <param name="brandId">The brand of the webhook</param>
		Task DeleteSubscription(Guid brandId, Guid id);
	}
}