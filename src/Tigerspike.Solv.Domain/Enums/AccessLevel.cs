
using System.ComponentModel.DataAnnotations;

namespace Tigerspike.Solv.Domain.Enums
{
	public enum AccessLevel
	{
		None = 0,
		InternalAgent = 1,
		RegularSolver = 2,
		SuperSolver = 3,
		Client = 4,
		Admin = 5,
	}
}