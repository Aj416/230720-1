using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Tigerspike.Solv.Core.Constants;

namespace Tigerspike.Solv.Core.Extensions
{
	public static class IdentityExtensions
	{
		/// <summary>
		/// Return the user Id of the logged in user.
		/// </summary>
		public static Guid GetId(this ClaimsPrincipal claimsPrincipal)
		{
			if (claimsPrincipal == null)
			{
				throw new ArgumentNullException(nameof(claimsPrincipal));
			}

			// Auth0 adds automatically a prefix to the id.
			var id = claimsPrincipal.Identity.Name.Contains("auth0|")
				? claimsPrincipal.Identity.Name.Substring("auth0|".Length)
				: claimsPrincipal.Identity.Name;

			return Guid.Parse(id);
		}

		public static string GetEmail(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.GetClaimValue("email") ?? claimsPrincipal.GetClaimValue(ClaimTypes.Email);

		public static string GetClaimValue(this ClaimsPrincipal claimsPrincipal, string key)
		{
			if (claimsPrincipal == null)
			{
				throw new ArgumentNullException(nameof(claimsPrincipal));
			}
			return claimsPrincipal.FindFirst(key)?.Value;
		}

		public static List<string> GetRoles(this ClaimsPrincipal claimsPrincipal)
		{
			if (claimsPrincipal == null)
			{
				throw new ArgumentNullException(nameof(claimsPrincipal));
			}

			return claimsPrincipal.Claims
				.Where(c => c.Type == ClaimTypes.Role)
				.Select(c => c.Value)
				.ToList();
		}

		public static Guid GetBrandId(this ClaimsPrincipal claimsPrincipal)
		{
			if (claimsPrincipal == null)
			{
				throw new ArgumentNullException(nameof(claimsPrincipal));
			}

			var claim = GetClaimValue(claimsPrincipal, SolvClaimTypes.BrandId);
			return Guid.Parse(claim);
		}

		public static bool HasTokenForTicket(this ClaimsPrincipal claimsPrincipal, Guid ticketId) => GetCustomerTicketId(claimsPrincipal) == ticketId.ToString();

		/// <summary>
		/// Return the ticket id extracted from the token claims.
		/// </summary>
		public static string GetCustomerTicketId(this ClaimsPrincipal claimsPrincipal)
		{
			claimsPrincipal = claimsPrincipal ?? throw new ArgumentNullException(nameof(claimsPrincipal));
			var tokenTicketId = GetClaimValue(claimsPrincipal, ClaimTypes.Sid);
			return tokenTicketId;
		}
	}
}