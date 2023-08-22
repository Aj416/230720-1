namespace Tigerspike.Solv.Api.Models.Brand
{
	/// <summary>
	/// Brand asset
	/// </summary>
	public class BrandAssetModel
	{
		/// <summary>
		/// Public url to the asset
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public BrandAssetModel(string url)
		{
			Url = url;
		}
	}
}