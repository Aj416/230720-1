using System;

namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// The received messenger model.
	/// </summary>
	public class MessengerReceivedModel
	{
		/// <summary>
		/// App object
		/// </summary>
		public MessengerApplicationModel App { get; set; }

		/// <summary>
		/// Events
		/// </summary>
		public MessengerEventModel[] Events { get; set; }

		/// <summary>
		/// Webhook
		/// </summary>
		public MessengerWebHookModel Webhook { get; set; }
	}

	/// <summary>
	/// Messenger App
	/// </summary>
	public class MessengerApplicationModel
	{
		/// <summary>
		/// The application id.
		/// </summary>
		public string Id { get; set; }
	}

	/// <summary>
	/// Messenger event model.
	/// </summary>
	public class MessengerEventModel
	{
		/// <summary>
		/// Event id.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Event type.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Event created at.
		/// </summary>
		public DateTime CreatedAt { get; set; }

		/// <summary>
		/// The received payload.
		/// </summary>
		public MessengerEventPayloadModel Payload { get; set; }
	}

	/// <summary>
	/// The payload model.
	/// </summary>
	public class MessengerEventPayloadModel
	{
		/// <summary>
		/// The payload conversation model.
		/// </summary>
		public MessengerConversationModel Conversation { get; set; }

		/// <summary>
		/// The payload message model.
		/// </summary>
		public MessengerMessageModel Message { get; set; }
	}

	/// <summary>
	/// The messenger message model.
	/// </summary>
	public class MessengerMessageModel
	{
		/// <summary>
		/// The message id to be used for one time delivery.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// The date time the message was received.
		/// </summary>
		public DateTime Received { get; set; }

		/// <summary>
		/// The message author.
		/// </summary>
		public MessengerAuthorModel Author { get; set; }

		/// <summary>
		/// The message content.
		/// </summary>
		public MessengerContentModel Content { get; set; }
	}

	/// <summary>
	/// The message content model.
	/// </summary>
	public class MessengerContentModel
	{
		/// <summary>
		/// The message type.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// The message text.
		/// </summary>
		public string Text { get; set; }
	}

	/// <summary>
	/// The message author model.
	/// </summary>
	public class MessengerAuthorModel
	{
		/// <summary>
		/// The user id.
		/// </summary>
		public string UserId { get; set; }

		/// <summary>
		/// The user display name in the messenger.
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// The author type.
		/// </summary>
		public string Type { get; set; }
	}

	/// <summary>
	/// The conversation model.
	/// </summary>
	public class MessengerConversationModel
	{
		/// <summary>
		/// The conversation id.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// The conversation type.
		/// </summary>
		public string Type { get; set; }
	}

	/// <summary>
	/// The webhook model.
	/// </summary>
	public class MessengerWebHookModel
	{
		/// <summary>
		/// The webhook id.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// The webhook version.
		/// </summary>
		public string Version { get; set; }
	}
}