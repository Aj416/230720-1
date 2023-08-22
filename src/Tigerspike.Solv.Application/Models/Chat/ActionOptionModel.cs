using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Chat
{
	public class ActionOptionModel
	{
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