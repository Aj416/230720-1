using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Ticket
{
	public class TicketImportModel
	{
		public Guid Id { get; set; }
		public string UploadDateText => UploadDate.ToString("dd MMM yy, hh:mm:ss");

		public ImportStatusEnum Status =>
			Total <= Imported + Failed ? ImportStatusEnum.Complete : ImportStatusEnum.Processing;

		public DateTime UploadDate { get; set; }
		public string CloseDateText => ClosedDate.ToString("dd MMM yy, hh:mm:ss");
		public DateTime ClosedDate { get; set; }
		public int Total { get; set; }
		public string UserFullName { get; set; }
		public int Imported { get; set; }
		public int Failed { get; set; }
		public IEnumerable<string> Tags { get; set; }
		public decimal Price { get; set; }
	}
}