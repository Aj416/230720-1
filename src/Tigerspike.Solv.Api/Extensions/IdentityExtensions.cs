using System.Security.Claims;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Api.Extensions
{
	/// <summary>
	/// ExceptionMiddlewareExtensions
	/// </summary>
	public static class IdentityExtensions
	{
		/// <summary>
		/// Get the level of the user
		/// </summary>
		public static TicketLevel GetLevel(this ClaimsPrincipal user) => user.IsInRole(SolvRoles.SuperSolver) ? TicketLevel.SuperSolver : TicketLevel.Regular;
	}
}