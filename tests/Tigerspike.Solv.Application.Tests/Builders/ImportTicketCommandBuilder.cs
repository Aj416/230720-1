using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Commands.Ticket;

namespace Tigerspike.Solv.Application.Tests
{
	public static class ImportTicketCommandBuilder
	{

		public static ImportTicketCommand Build(
			Guid importId = new Guid(), 
			string rawInput = null,
			Guid brandId = new Guid(), 
			string referenceId = null, 
			string source = null, 
			string question = null, 
			string customerFirstName = null, 
			string customerLastName = null, 
			string customerEmail = null, 
			string advocateEmail = null, 
			DateTime createdDate = new DateTime(), 
			DateTime? assignedDate = null, 
			DateTime? solvedDate = null, 
			DateTime? closedDate = null, 
			decimal? price = null, 
			int? complexity = null, 
			int? csat = null, 
			IReadOnlyDictionary<string, string> metadata = null, 
			Guid[] tags = null
		)
		{
			return new ImportTicketCommand(
				 importId,
				 rawInput,
				 brandId,
				 referenceId,
				 source,
				 question,
				 customerFirstName,
				 customerLastName,
				 customerEmail,
				 advocateEmail,
				 createdDate,
				 assignedDate,
				 solvedDate,
				 closedDate,
				 price,
				 complexity,
				 csat,
				 metadata,
				 tags
			);
		}

	}
}