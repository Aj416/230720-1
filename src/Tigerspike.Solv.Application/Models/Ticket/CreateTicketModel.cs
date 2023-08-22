using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Ticket
{
	public class CreateTicketModel
	{
		public string Question { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string ReferenceId { get; set; }
		public string Source { get; set; }
		public TicketTransportType TransportType { get; set; }
		public Dictionary<string, string> Metadata { get; set; }
		public Dictionary<Guid, Guid?> ProbingAnswers { get; set; }

		public virtual void Sanitize()
		{
			Email = Email.ToLowerInvariant();
			FirstName = FirstName.Trim().StripHtml().ToTitleCase();
			LastName = LastName.Trim().StripHtml().ToTitleCase();
			Question = Question.Trim().StripHtml(false, true);

			if (Metadata != null && Metadata.Count > 0)
			{
				var sanitized = new Dictionary<string, string>();
				foreach (var keyValuePair in Metadata)
				{
					sanitized.Add(keyValuePair.Key.RemoveAllSpecialCharacters().ToCamelCase(),
						keyValuePair.Value.Trim().StripHtml());
				}

				Metadata = sanitized;
			}
		}
	}
}