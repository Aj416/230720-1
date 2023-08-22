using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Models
{
	public class Tag
	{
		/// <summary>
		/// Gets or sets the Id of the tag
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the brand id of the tag.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Gets or sets the name of the tag
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets whether the tag is enabled
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// The action related to the tag
		/// </summary>
		public TicketFlowAction? Action { get; private set; }

		/// <summary>
		/// Gets or sets level of tag
		/// </summary>
		public int? Level { get; set; }

		/// <summary>
		/// Gets or sets parent tag id
		/// </summary>
		public Guid? ParentTagId { get; set; }

		/// <summary>
		/// Gets or sets parent tag.
		/// </summary>
		public Tag ParentTag { get; set; }

		/// <summary>
		/// Gets or sets list of subtag.
		/// </summary>
		public ICollection<Tag> SubTags { get; set; }

		/// <summary>
		/// Determines if diagnosis enabled.
		/// </summary>
		public bool? DiagnosisEnabled { get; set; }

		/// <summary>
		/// Determines if SPOS notification is enabled.
		/// </summary>
		public bool? SposNotificationEnabled { get; set; }

		/// <summary>
		/// Tag Description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// The disable status of the tag after the ticket is closed
		/// </summary>
		public bool L1PostClosureDisable { get; set; }

		/// <summary>
		/// The disable status of the tag after the ticket is closed
		/// </summary>
		public bool L2PostClosureDisable { get; set; }

		/// <summary>
		/// The default constructor
		/// </summary>
		public Tag() { }

		/// <summary>
		/// The parameterized constructor
		/// </summary>
		public Tag(Guid brandId, string name, TicketFlowAction? action = null) =>
			(BrandId, Name, Action) = (brandId, name, action);

		/// <summary>
		/// Check for the validity of the tag
		/// </summary>
		public bool IsValid() => new Regex("([^a-z-])").IsMatch(Name) == false;

	}
}
