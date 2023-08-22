using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models
{
	public class CreateTagModel
	{
		/// <summary>
		/// The brand Id
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Gets or sets tag name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets tag level.
		/// </summary>
		public TicketLevel? Level { get; set; }

		/// <summary>
		/// Gets or sets tag flow.
		/// </summary>
		public TicketFlowAction? Action { get; set; }

		/// <summary>
		/// Gets or sets sub tags.
		/// </summary>
		public IList<CreateTagModel> SubTags { get; set; } = new List<CreateTagModel>();

		/// <summary>
		/// Determines if diagnosis enabled.
		/// </summary>
		public bool? DiagnosisEnabled { get; set; }

		/// <summary>
		/// Determines if SPOS notification is enabled.
		/// </summary>
		public bool? SposNotificationEnabled { get; set; }
	}
}