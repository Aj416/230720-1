using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Enums;

namespace Tigerspike.Solv.Application.Interfaces
{
	/// <summary>
	/// Service exposes Solv specific authorization methods
	/// </summary>
	public interface ISolvAuthorizationService
	{
		/// <summary>
		/// Returns whether user is authorized to interact with the tickets (role-dependant logic is applied)
		/// </summary>
		/// <param name="user">The user claims principal</param>
		/// <param name="ticketId">The one or more ticket id</param>
		/// <returns>True is user is authorized for the ticket, false otherwise</returns>
		Task<bool> IsAuthorizedToViewTicket(ClaimsPrincipal user, params Guid[] ticketIds);

		/// <summary>
		/// Returns whether user is authorized to edit the tickets (role-dependant logic is applied)
		/// </summary>
		/// <param name="user">The user claims principal</param>
		/// <param name="ticketId">The one or more ticket id</param>
		/// <returns>True is user is authorized for the ticket, false otherwise</returns>
		Task<bool> IsAuthorizedToEditTicket(ClaimsPrincipal user, SolvOperationEnum operation, params Guid[] ticketIds);

		/// <summary>
		/// return false if any of the provided brand cannot be viewed by the 
		/// </summary>
		Task<bool> IsAuthorizedToViewBrands(ClaimsPrincipal user, Guid[] brandsId);
	}
}