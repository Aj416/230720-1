namespace Tigerspike.Solv.Core.Events
{
	public interface IEventStore
	{
		void Save<T>(T theEvent)where T : Event;
	}
}