using System;

namespace Tigerspike.Solv.Domain.Models
{
	public class TicketMetadataItem
	{
		public Guid TicketId { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
		public Guid? BrandFormFieldId { get; set; }
		public int? Order { get; set; }

		public TicketMetadataItem() { }

		public TicketMetadataItem(string key, string value, Guid? brandFormFieldId = null, int? order = null)
		{
			Key = key;
			Value = value;
			BrandFormFieldId = brandFormFieldId;
			Order = order;
		}
	}
}
