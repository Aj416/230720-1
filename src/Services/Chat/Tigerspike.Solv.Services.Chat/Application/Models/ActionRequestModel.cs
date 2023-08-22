using System.Collections.Generic;

namespace Tigerspike.Solv.Services.Chat.Application.Models
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