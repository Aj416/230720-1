namespace Tigerspike.Solv.Domain.Enums
{
	/// <summary>
	/// Types of chat items.
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// Chat item message that is entered by a user.
		/// </summary>
		Message = 1,

		/// <summary>
		/// Chat item message that is generated by the system.
		/// </summary>
		SystemMessage = 2,

		///<summary>
		/// Chat item that represents a status change
		/// </summary>
		StatusChange = 3,

		/// <summary>
		/// Chat item that represents a file that has been successfully uploaded.
		/// </summary>
		Attachment = 4,

		///<summary>
		/// Chat item that represents an action
		/// </summary>
		Action = 5,
	}
}
