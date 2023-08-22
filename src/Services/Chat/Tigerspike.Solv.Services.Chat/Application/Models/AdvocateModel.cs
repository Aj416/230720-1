using System;

namespace Tigerspike.Solv.Services.Chat.Application.Models
{
	public class AdvocateModel
	{
		public AdvocateModel(Guid id, string firstName, decimal csat)
		{
			Id = id;
			FirstName = firstName;
			Avatar = firstName != null ? firstName.Substring(0, 1) : null;
			Csat = csat;
		}
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string Avatar { get; set; }
		public decimal Csat { get; set; }
	}
}