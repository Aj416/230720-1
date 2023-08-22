using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class ImportTicketCommand : Command<Guid?>
	{
		public ImportTicketCommand(Guid importId, string rawInput, Guid brandId, string referenceId, string source, string question, string customerFirstName, string customerLastName, string customerEmail, string advocateEmail, DateTime createdDate, DateTime? assignedDate, DateTime? solvedDate, DateTime? closedDate, decimal? price, int? complexity, int? csat, IReadOnlyDictionary<string, string> metadata, Guid[] tags)
		{
			ImportId = importId;
			RawInput = rawInput;
			BrandId = brandId;
			ReferenceId = referenceId;
			Source = source;
			Question = question;
			CustomerFirstName = customerFirstName;
			CustomerLastName = customerLastName;
			CustomerEmail = customerEmail;
			AdvocateEmail = advocateEmail;
			CreatedDate = createdDate;
			AssignedDate = assignedDate;
			SolvedDate = solvedDate;
			ClosedDate = closedDate;
			Price = price;
			Complexity = complexity;
			Csat = csat;
			Metadata = metadata;
			Tags = tags;
		}

		/// <summary>
		/// Id of the import process associated with the imported ticket
		/// </summary>
		public Guid ImportId { get; }

		/// <summary>
		/// Raw input record data as provided in the import source (e.g. raw csv line)
		/// </summary>
		public string RawInput { get; }

		/// <summary>
		/// The brand id to which the ticket is to be imported
		/// </summary>
		public Guid BrandId { get; }

		/// <summary>
		/// Unique reference id of the imported ticket
		/// </summary>
		public string ReferenceId { get; }

		/// <summary>
		/// Source of the imported ticket (optional, e.g. Chat or Web form)
		/// </summary>
		public string Source { get; }

		public string Question { get; }
		public string CustomerFirstName { get; }
		public string CustomerLastName { get; }
		public string CustomerEmail { get; }

		/// <summary>
		/// Advocate/Solver email, optional, if provided, the ticket would be assigned to that Advocate/Solver
		/// </summary>
		public string AdvocateEmail { get; }

		/// <summary>
		/// Creation date of the ticket
		/// </summary>
		public DateTime CreatedDate { get; }

		/// <summary>
		/// Assignment to the advocate/solver date of the ticket, if not provided, ticket won't be marked as assigned
		/// </summary>
		public DateTime? AssignedDate { get; }

		/// <summary>
		/// Solving date of the ticket, optional, if not provided, ticket won't be marked as solved
		/// </summary>
		public DateTime? SolvedDate { get; }

		/// <summary>
		/// Clsoing date of the ticket, optional, if not provided, ticket won't be closed
		/// </summary>
		public DateTime? ClosedDate { get; }

		/// <summary>
		/// Price to be set on the ticket, if not provided, then default price for the whole import process would be used
		/// </summary>
		/// <value></value>
		public decimal? Price { get; }

		public int? Complexity { get; }
		public int? Csat { get; }

		/// <summary>
		/// Metadata of the ticket, provided as key-value pair dictionary, optional
		/// </summary>
		public IReadOnlyDictionary<string, string> Metadata { get; }

		/// <summary>
		/// Tags to be assigned to the ticket, optional
		/// </summary>
		public Guid[] Tags { get; }

		public override bool IsValid() => ImportId != Guid.Empty; // the rest of the validation is at ImportTicketCommandValidator
	}
}