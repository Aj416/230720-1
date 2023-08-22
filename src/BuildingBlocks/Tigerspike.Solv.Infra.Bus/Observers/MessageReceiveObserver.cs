using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Tigerspike.Solv.Infra.Bus.Observers
{
	public class MessageReceiveObserver : IReceiveObserver
	{
		private readonly ILogger _logger;

		public MessageReceiveObserver(ILogger logger) => _logger = logger;

		public async Task PreReceive(ReceiveContext context)
		{
			_logger.LogDebug("A message is before the receive stage. Has been delivered to at least one consumer: {isDelivered}", context.IsDelivered);
			await context.ReceiveCompleted;
		}

		public async Task PostReceive(ReceiveContext context)
		{
			_logger.LogDebug("A message is past the receive stage. Has been delivered to at least one consumer: {isDelivered}", context.IsDelivered);
			await context.ReceiveCompleted;
		}

		public async Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
		{
			_logger.LogDebug("A message is consumed for type {messageType}. Consumer type: {consumerType}, duration in milis: {duration}", context.Message.GetType(), consumerType, duration.TotalMilliseconds);
			await context.ConsumeCompleted;
		}

		public async Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType,
			Exception exception) where T : class
		{
			_logger.LogError(exception, "A fault is consumed for type {messageType}. Consumer type: {consumerType}, duration in milis: {duration}", context.Message.GetType(), consumerType, duration.TotalMilliseconds);
			await context.ConsumeCompleted;
		}

		public async Task ReceiveFault(ReceiveContext context, Exception exception)
		{
			_logger.LogError(exception, "A fault is received with exception");
			await context.ReceiveCompleted;
		}
	}
}