using System.ComponentModel.DataAnnotations;

namespace Tigerspike.Solv.Services.WebHook.Enums
{
	/// <summary>
	/// The user types
	/// </summary>
	public enum UserType
	{
		/// <summary>
		/// An unknown user
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// A solver
		/// </summary>
		[Display(Name = "Solver")]
		Advocate = 1,

		/// <summary>
		/// A customer
		/// </summary>
		Customer = 2,

		/// <summary>
		/// The system (used for system messages)
		/// </summary>
		System = 3,

		/// <summary>
		/// The super solver
		/// </summary>
		SuperSolver = 4,

		/// <summary>
		/// The bot (used for automated bot messages)
		/// </summary>
		SolvyBot = 5,
	}
}
