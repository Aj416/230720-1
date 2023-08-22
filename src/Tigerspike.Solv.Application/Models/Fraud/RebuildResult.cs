namespace Tigerspike.Solv.Application.Models.Fraud
{
	public class RebuildResult
	{
		public string IndexName { get; set; }
		public long SourceDocuments { get; set; }
		public long IndexedDocuments { get; set; }
	}
}
