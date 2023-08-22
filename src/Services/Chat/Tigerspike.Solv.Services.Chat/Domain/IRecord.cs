using ServiceStack.DataAnnotations;

namespace Tigerspike.Solv.Services.Chat.Domain
{
	public interface IRecord
	{
		[HashKey]
		public string HashKey { get; }

		[RangeKey]
		public string RangeKey { get; }

		public string RecordType { get; }
	}
}