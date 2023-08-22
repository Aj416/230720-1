using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Models
{
	public class TicketImport
	{
		/// <summary>
		/// The identifier.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// File name of uploded file
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// Uploded file type
		/// </summary>
		public FileTypeEnum FileType { get; set; }

		/// <summary>
		/// List Uploded on
		/// </summary>
		public DateTime UploadDate { get; set; }

		/// <summary>
		/// User Id of file uploder
		/// </summary>
		public Guid UserId { get; set; }

		/// <summary>
		/// Ticket closed date
		/// </summary>
		public DateTime ClosedDate { get; set; }

		/// <summary>
		/// Ticket price
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// Ticket fee
		/// </summary>
		public decimal Fee{get;set;}

		/// <summary>
		/// Brand ID of the uploded list
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Number of tickets to be imported
		/// </summary>
		public int TicketCount { get; set; }

		/// <summary>
		/// User details
		/// </summary>
		public User User { get; set; }

		/// <summary>
		/// List of tag associted with ticket
		/// </summary>
		public ICollection<TicketImportTag> Tags { get; set; }

		/// <summary>
		/// List of Failures ticket
		/// </summary>
		public ICollection<TicketImportFailure> Failures { get; private set; }

		/// <summary>
		/// List of Success ticket
		/// </summary>
		public ICollection<Ticket> Tickets { get; private set; }

		public void AddFailure(TicketImportFailure failure)
		{
			Failures ??= new List<TicketImportFailure>();
			Failures.Add(failure);
		}
	}
}