namespace Tigerspike.Solv.Core.Events
{
	public class InMemoryEventStore : IEventStore
	{
		/// <inheritdoc />
		public void Save<T>(T theEvent)where T : Event
		{
			// Ignore
		}
	}
}