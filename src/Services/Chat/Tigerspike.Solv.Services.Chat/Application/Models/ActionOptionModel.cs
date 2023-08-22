using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Services.Chat.Application.Models
{
	public class ActionOptionModel
	{
		/// <summary>
		/// The action option id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The action id.
		/// </summary>
		public Guid ChatActionId { get; set; }

		/// <summary>
		/// Label of the option
		/// </summary>
		public string Label { get; set; }

		/// <summary>
		/// Value of the option
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Is option selected (chosen)
		/// </summary>
		public bool IsSelected { get; set; }

		/// <summary>
		/// Whether it is suggested option to chose
		/// </summary>
		public bool IsSuggested { get; set; }

		/// <summary>
		/// Side effects for action
		/// </summary>
		public IEnumerable<SideEffectModel> SideEffects { get; set; }
	}
}