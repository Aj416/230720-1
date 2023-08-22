using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Chat
{
	public class ActionRequestModel
	{
		/// <summary>
		/// Options selected for action
		/// </summary>
		public IEnumerable<string> Options { get; set; }

		/// <summary>
		/// Customer feedback for specific ticket.
		/// </summary>
		public string AdditionalFeedBack { get; set; }
	}
}