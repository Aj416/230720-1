using System;

namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// Change Name Model
	/// </summary>
	public class ChangeNameModel
	{
		/// <summary>
		/// The first name to be updated.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// The last name to be updated.
		/// </summary>
		public string LastName { get; set; }
	}
}