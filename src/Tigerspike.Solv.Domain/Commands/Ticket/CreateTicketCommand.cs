using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Validators;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class CreateTicketCommand : Command<Guid?, CreateTicketCommandValidator>
	{
		public string FirstName { get; }

		public string LastName { get; }

		public string Email { get; }

		public string Question { get; }

		public Guid BrandId { get; }

		public string ReferenceId { get; }

		public string ThreadId { get; }

		public string Source { get; }

		public Guid? PracticingAdvocateId { get; set; }

		public TicketTransportType TransportType { get; set; }

		public IReadOnlyDictionary<string, string> Metadata { get; set; }
		
		public IReadOnlyDictionary<Guid, Guid?> ProbingAnswers { get; set; }

		public CreateTicketCommand(string firstName, string lastName, string email, string question, Guid brandId, TicketTransportType transportType, string referenceId = null, string threadId = null, string source = null, Guid? practicingAdvocateId = null, IReadOnlyDictionary<string, string> metadata = null, IReadOnlyDictionary<Guid, Guid?> probingAnswers = null)
		{
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			Question = question;
			BrandId = brandId;
			PracticingAdvocateId = practicingAdvocateId;
			ReferenceId = referenceId;
			ThreadId = threadId;
			Source = source;
			TransportType = transportType;
			Metadata = metadata;
			ProbingAnswers = probingAnswers;
		}
	}
}