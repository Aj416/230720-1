using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchAdvocateInfoResult : IResult
	{
		/// <summary>
		/// List of advocate info.
		/// </summary>
		IEnumerable<IAdvocateInfo> AdvocateInfo { get; set; }
	}

	public interface IAdvocateInfo
	{
		/// <summary>
		/// Advocate Identifier.
		/// </summary>
		Guid AdvocateId { get; set; }

		/// <summary>
		/// Advocate first name.
		/// </summary>
		string FirstName { get; set; }

		/// <summary>
		/// Advocate last name.
		/// </summary>
		string LastName { get; set; }

		/// <summary>
		/// Advocate phone number.
		/// </summary>
		string Phone { get; set; }

		/// <summary>
		/// Advocate email id.
		/// </summary>
		string Email { get; set; }

		/// <summary>
		/// Advocate country code.
		/// </summary>
		string CountryCode { get; set; }

		/// <summary>
		/// Advoacte Status.
		/// </summary>
		int AdvocateStatus { get; set; }

		/// <summary>
		/// Payment account identifier.
		/// </summary>
		string PaymentAccountId { get; set; }
	}

}
