using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Services
{
	public class CreateTagCommand : Command
	{
		/// <summary>
		/// The brand Id
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Tag name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Ticket flow action to take upon selecting this reason
		/// </summary>
		public TicketFlowAction? Action { get; set; }

		/// <summary>
		/// Determines level of tag.
		/// </summary>
		public TicketLevel? Level { get; set; }

		/// <summary>
		/// Gets or sets list of sub tags.
		/// </summary>
		public IList<CreateTagCommand> SubTags { get; set; }

		/// <summary>
		/// Determines if diagnosis enabled.
		/// </summary>
		public bool? DiagnosisEnabled { get; set; }

		/// <summary>
		/// Determines if SPOS notification is enabled.
		/// </summary>
		public bool? SposNotificationEnabled { get; set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected CreateTagCommand() => SubTags = new List<CreateTagCommand>();

		public override bool IsValid() => BrandId != Guid.Empty && new Regex("([^a-z-])").IsMatch(Name) == false;
	}
}