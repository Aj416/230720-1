using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Chat
{
	public interface IChatAction
	{
		/// <summary>
		/// Type of action (question, feedback requests etc.)
		/// </summary>
		public int Type { get; set; }

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
		public List<IChatActionOption> Options { get; set; }
	}
}