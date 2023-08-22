using System;
using System.Collections.Generic;
using Tigerspike.Solv.Chat.Enums;

namespace Tigerspike.Solv.Services.Chat.Application.Models
{
	public class ActionModel
	{
		/// <summary>
		/// The id of the action.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Type of action (question, feedback requests etc.)
		/// </summary>
		public ActionType Type { get; set; }

		/// <summary>
		/// Has action been finalized
		/// </summary>
		public bool IsFinalized { get; set; }

		/// <summary>
		/// Is action blocking chat until acted upon
		/// </summary>
		public bool IsBlocking { get; set; }

		/// <summary>
		/// Options available in action
		/// </summary>
		public IEnumerable<ActionOptionModel> Options { get; set; }
	}
}