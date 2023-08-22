namespace Tigerspike.Solv.Core.Extensions
{
	public static class BooleanExtensions
	{
		public static string ToLower(this bool value) => value.ToString().ToLowerInvariant();
	}
}