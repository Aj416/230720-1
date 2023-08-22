using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Core.Mediator
{
	public sealed class InMemoryBus : IMediatorHandler
	{
		private readonly IMediator _mediator;

		public InMemoryBus(IMediator mediator)
		{
			_mediator = mediator;
		}

		public Task SendCommand(Command command)
		{
			return _mediator.Send(command);
		}

		public Task<T> SendCommand<T>(Command<T> command)
		{
			return _mediator.Send(command);
		}

		public Task RaiseEvent<T>(T @event) where T : Event
		{
			return _mediator.Publish(@event);
		}
	}
}