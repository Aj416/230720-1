using System;

namespace Tigerspike.Solv.Application.Models.Ticket
{
	/// <summary>
	/// The advocate information that the customer can see.
	/// </summary>
	public class CustomerAdvocateModel
	{
		public Guid Id { get; set; }

		public string FirstName { get; set; }
		public string Avatar { get; set; }

		public bool InternalAgent { get; set; }

		/// <summary>
		/// The average Csat that this advocate has received for a specific brand.
		/// </summary>
		public decimal? Csat { get; set; }

		public CustomerAdvocateModel(Guid id, string firstName, bool internalAgent, decimal? csat = null)
		{
			(Id, FirstName, InternalAgent, Csat) = (id, firstName, internalAgent, csat);
			Avatar = firstName != null ? firstName.Substring(0, 1) : null;
		}
	}
}
