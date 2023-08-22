using System.Collections.Generic;

namespace Tigerspike.Solv.Core.Configuration
{
	public class StorageOptions
	{
		public const string SectionName = "Storage";

		public string ExportBucket { get; set; }
		public string BrandAssetsBucket { get; set; }
		public string BrandAssetsUrlFormat { get; set; }
		public int BrandAssetsMaxSize { get; set; }
		public IEnumerable<string> BrandAssetsAllowedExtensions { get; set; }
		public string ExportScheduledPrefix { get; set; }
		public string TicketsImportBucket { get; set; }
		public string TicketsImportPrefix { get; set; }
		public int TicketsImportMaxSize { get; set; }
		public IEnumerable<string> TicketsImportAllowedExtensions { get; set; }
	}
}