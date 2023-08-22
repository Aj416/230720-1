using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Tigerspike.Solv.Application.Enums;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Enums;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Messaging.WebHook;

namespace Tigerspike.Solv.Application.Services
{
	public class WebHookService : IWebHookService
	{
		private readonly IMapper _mapper;
		private readonly IMediatorHandler _mediator;
		private readonly IBus _bus;
		private readonly IFeatureManager _featureManager;
		private readonly ITimestampService _timestampService;
		private readonly BusOptions _busOptions;

		public WebHookService(

			IMapper mapper,
			IMediatorHandler mediator,
			IBus bus,
			IOptions<BusOptions> busOptions,
			IFeatureManager featureManager,
			ITimestampService timestampService)
		{
			_mapper = mapper;
			_mediator = mediator;
			_bus = bus;
			_featureManager = featureManager;
			_timestampService = timestampService;
			_busOptions = busOptions.Value;
		}

		/// <inheritdoc/>
		public async Task CreateSubscription(Guid brandId, Guid? userId, CreateWebHookModel model)
		{
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.WebHook}"));
			await endpoint.Send<ICreateSubscriptionCommand>(new
			{
				BrandId = brandId,
				UserId = userId,
				Url = model.Url,
				Body = model.Body,
				Verb = model.Verb,
				ContentType = model.ContentType,
				Secret = model.Secret,
				Authorization = model.Authorization,
				EventType = (int)model.EventType
			});
		}

		/// <inheritdoc/>
		public async Task DeleteSubscription(Guid brandId, Guid id)
		{
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.WebHook}"));
			await endpoint.Send<IDeleteSubscriptionCommand>(new
				{BrandId = brandId, Id = id});
		}

		/// <inheritdoc/>
		public async Task Publish(Guid brandId, WebHookEventTypes webHookEventType, IDictionary<string, object> payload)
		{
			if (await _featureManager.IsEnabledAsync(nameof(FeatureFlags.WebHooks)))
			{
				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.WebHook}"));
				await endpoint.Send<IStartWebHookTriggerCommand>(new
					{BrandId = brandId, EventType = (int) webHookEventType, Payload = payload});
			}
		}
	}
}