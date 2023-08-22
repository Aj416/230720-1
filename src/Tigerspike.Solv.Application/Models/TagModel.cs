using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models
{
	public class TagModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public TicketFlowAction? Action { get; set; }
		public IList<TagModel> SubTags { get; set; } = new List<TagModel>();
		public string ToolTip { get; set; }
	}
}