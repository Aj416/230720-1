
namespace Tigerspike.Solv.Infra.Data.SeedData.Factories
{
	public class RejectionReasonFactory
	{
		public static string[] GetList()
		{
			return new string[]
			{
				"Don't understand", "Too difficult", "Not relevant to me", "Technical issue",
				"Don't like the price"
			};
		}
	}
}