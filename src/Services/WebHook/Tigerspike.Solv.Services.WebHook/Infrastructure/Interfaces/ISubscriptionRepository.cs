using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Services.WebHook.Domain;
using Tigerspike.Solv.Services.WebHook.Enums;

namespace Tigerspike.Solv.Services.WebHook.Infrastructure.Interfaces
{
	public interface ISubscriptionRepository
	{
		/// <summary>
		/// Adds or updates the subscription.
		/// </summary>
		/// <param name="subscription">The subscription.</param>
		/// <returns>The subscription.</returns>
		Subscription AddOrUpdateSubscription(Subscription subscription);

		/// <summary>
		/// Deletes a subscription by id.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <param name="id">The subscription id.</param>
		void DeleteSubscription(Guid brandId, Guid id);

		/// <summary>
		/// Gets a subscription by id.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <param name="id">The subscription id.</param>
		/// <returns>The subscription.</returns>
		Subscription GetSubscription(Guid brandId, Guid id);

		/// <summary>
		/// Gets the list subscriptions for the brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <param name="eventType">The event type.</param>
		/// <returns>The list of subscriptions.</returns>
		Task<List<SubscriptionEventLocalIndex>> GetSubscriptions(Guid brandId, int eventType);
	}
}