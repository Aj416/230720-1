using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class CreateTicketsExportCommand
	{
		/// <summary>
		/// The email address of export recipient
		/// </summary>
		public string RecipientAddress { get; private set; }

		/// <summary>
		/// From timestamp range for the export
		/// </summary>
		public DateTime? From { get; private set; }

		/// <summary>
		/// To timestamp range for the export
		/// </summary>
		public DateTime? To { get; private set; }

		/// <summary>
		/// Filters the tickets by the brand specified.
		/// </summary>
		public Guid? BrandId { get; private set; }

		/// <summary>
		/// Indicates whether this was scheduled by the automated process.
		/// </summary>
		public bool Scheduled { get; private set; }

		/// <summary>
		/// Indicates whether this trigged by the admin
		/// </summary>
		public CsvExportSource TriggeredBy { get; private set;}

		/// <summary>
		/// The empty constructor for the bus activator
		/// </summary>
		public CreateTicketsExportCommand()
		{
		}

		/// <summary>
		/// The constructor.
		/// </summary>
		public CreateTicketsExportCommand(string recipientAddress, DateTime? from, DateTime? to,
			Guid? brandId, CsvExportSource triggeredBy, bool scheduled = false)
		{
			RecipientAddress = recipientAddress;
			From = from;
			To = to;
			BrandId = brandId;
			Scheduled = scheduled;
			TriggeredBy = triggeredBy;
		}

	}
}