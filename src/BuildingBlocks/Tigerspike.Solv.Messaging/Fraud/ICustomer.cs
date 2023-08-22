using System;

namespace Tigerspike.Solv.Messaging.Fraud
{
	/// <summary>
	/// Customer data point.
	/// </summary>
	public interface ICustomer
	{
		/// <summary>
		/// Gets or sets TicketId.
		/// </summary>
		Guid TicketId { get; set; }

		/// <summary>
		/// Gets or sets Value.
		/// </summary>
		string Value { get; set; }

		/// <summary>
		/// Gets or sets Key of type CustomerDetail.
		/// </summary>
		CustomerDetail Key { get; set; }
	}

	public enum CustomerDetail
	{
		FullName,
		Email,
		Ip
	}
}