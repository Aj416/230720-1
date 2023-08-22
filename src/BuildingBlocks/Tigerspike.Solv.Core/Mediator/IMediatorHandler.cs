using System.Threading.Tasks;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Core.Mediator
{
	public interface IMediatorHandler
	{
		Task<T> SendCommand<T>(Command<T> command);
		Task SendCommand(Command command);
		Task RaiseEvent<T>(T @event)where T : Event;
	}
}