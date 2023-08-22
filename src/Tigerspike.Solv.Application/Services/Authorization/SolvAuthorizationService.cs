using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Enums;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Application.Services.Authorization
{
	public class SolvAuthorizationService : ISolvAuthorizationService
	{
		private readonly ITicketService _ticketService;
		private readonly IBrandService _brandService;

		public SolvAuthorizationService(ITicketService ticketService, IBrandService brandService) =>
			(_ticketService, _brandService) =
			(ticketService ?? throw new ArgumentNullException(nameof(ticketService)),
			brandService ?? throw new ArgumentNullException(nameof(brandService)));

		/// <inheritdoc/>
		public async Task<bool> IsAuthorizedToViewTicket(ClaimsPrincipal user, params Guid[] ticketIds)
		{
			return user.IsInRole(SolvRoles.Customer) ?
				user.HasTokenForTicket(ticketIds.Single()) :
				await _ticketService.CanView(user, ticketIds);
		}

		/// <inheritdoc/>
		public async Task<bool> IsAuthorizedToEditTicket(ClaimsPrincipal user, SolvOperationEnum operation, params Guid[] ticketIds)
		{
			if (user.IsInRole(SolvRoles.Admin) || user.IsInRole(SolvRoles.Client))
			{
				// Admin and clients can't manipulate tickets.
				return false;
			}

			if (user.IsInRole(SolvRoles.Customer))
			{
				if (!user.HasTokenForTicket(ticketIds.Single()))
				{
					return false;
				}
			}

			return await _ticketService.CanEdit(user, operation, ticketIds);
		}

		public async Task<bool> IsAuthorizedToViewBrands(ClaimsPrincipal user, Guid[] brandsId)
		{
			if (user.IsInRole(SolvRoles.Advocate) || user.IsInRole(SolvRoles.SuperSolver))
			{
				var result = (await _brandService.GetForAdvocate(user.GetId())).Select(b => b.Id);
				return brandsId.All(id => result.Contains(id));
			}

			// That's right, I'm lazy to check other roles because I only need advocate now :)
			// If you need other roles, too bad, you have to write it yourself.
			throw new NotSupportedException("Only advocate is supported now");
		}
	}
}