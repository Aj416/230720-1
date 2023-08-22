using System;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Application.Models.Fraud
{
	public class EmailInfo : ICustomer
	{
		public Guid TicketId { get; set; }
		public string Value { get; set; }
		public CustomerDetail Key { get; set; } = CustomerDetail.Email;
	}

	public class FullNameInfo : ICustomer
	{
		public Guid TicketId { get; set; }
		public string Value { get; set; }
		public CustomerDetail Key { get; set; } = CustomerDetail.FullName;
	}

		public class IpInfo : ICustomer
	{
		public Guid TicketId { get; set; }
		public string Value { get; set; }
		public CustomerDetail Key { get; set; } = CustomerDetail.Ip;
	}
}