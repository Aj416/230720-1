using System.Threading.Tasks;
using MassTransit;
using Tigerspike.Solv.Messaging.Notification;
using Tigerspike.Solv.Services.Notification.Application.Services;

namespace Tigerspike.Solv.Services.Notification.Application.Commands
{
	public class SendMessengerMessageConsumer : IConsumer<ISendMessengerMessageCommand>
	{
		private readonly IMessengerService _messengerService;

		public SendMessengerMessageConsumer(IMessengerService messengerService)
		{
			_messengerService = messengerService;
		}

		public Task Consume(ConsumeContext<ISendMessengerMessageCommand> context)
		{
			return _messengerService.PostMessage(context.Message.ConversationId, context.Message.Text);
		}
	}
}