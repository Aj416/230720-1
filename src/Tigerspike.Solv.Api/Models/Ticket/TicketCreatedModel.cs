using System;

namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// Ticket Complexity Model
	/// </summary>
	public class TicketCreatedModel
	{
		public Guid Id { get; private set; }
		public string Token { get; private set; }
		public string ChatUrl { get; private set; }

		public TicketCreatedModel(Guid id, string token, string chatUrl)
		{
			Id = id;
			Token = token;
			ChatUrl = chatUrl;
		}
	}
}