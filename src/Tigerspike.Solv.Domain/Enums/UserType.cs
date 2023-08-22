
using System.ComponentModel.DataAnnotations;

namespace Tigerspike.Solv.Domain.Enums
{
	public enum UserType
	{
		Unknown = 0,
		[Display(Name = "Solver")]
		Advocate = 1,
		Customer = 2,
		System = 3,
		SuperSolver = 4,
		SolvyBot = 5,
	}
}