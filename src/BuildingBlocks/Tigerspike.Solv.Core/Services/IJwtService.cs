using System;
using Tigerspike.Solv.Core.Models;

namespace Tigerspike.Solv.Core.Services
{
	public interface IJwtService
	{
		JwtModel CreateTokenForTicket(Guid ticketId, Guid userId);

		/// <summary>
		/// Generates a token for the sdk.
		/// </summary>
		/// <param name="applicationId">The application id.</param>
		/// <returns>The jwt token.</returns>
		JwtModel CreateSdkToken(string applicationId);
	}
}