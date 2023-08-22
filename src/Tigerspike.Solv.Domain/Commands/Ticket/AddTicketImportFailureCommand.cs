using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class AddTicketImportFailureCommand : Command<Unit>
	{
		public AddTicketImportFailureCommand(Guid importId, string rawInput, string failureReason)
		{
			ImportId = importId;
			RawInput = rawInput;
			FailureReason = failureReason;
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
		/// The reason for failure
		/// </summary>
		public string FailureReason { get; }

		public override bool IsValid() => ImportId != Guid.Empty;
	}
}