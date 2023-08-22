using Newtonsoft.Json;

namespace Tigerspike.Solv.Core.Email
{
	public static class SolvLiquidFilters
	{
		/// <summary>
		/// Format input into JSON value
		/// </summary>
		public static string Json(object input) => JsonConvert.SerializeObject(input);
		public static string Isodate(object input) => DotLiquid.StandardFilters.Date(input, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK");
	}
}