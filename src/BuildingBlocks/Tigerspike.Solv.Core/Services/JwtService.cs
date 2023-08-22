using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Models;

namespace Tigerspike.Solv.Core.Services
{
	public class JwtService : IJwtService
	{
		private readonly JwtOptions _jwtOptions;
		private readonly ITimestampService _timestampService;

		public JwtService(
			ITimestampService timestampService,
			IOptions<JwtOptions> jwtOptions)
		{
			_timestampService = timestampService ?? throw new ArgumentNullException(nameof(timestampService));
			_jwtOptions = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
		}

		/// <inheritdoc />
		public JwtModel CreateTokenForTicket(Guid ticketId, Guid userId)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
				new Claim("sub", userId.ToString()),
				new Claim(ClaimTypes.Sid, ticketId.ToString()),
				new Claim(ClaimTypes.Role, SolvRoles.Customer)
			};

			var expiryDate = _timestampService.GetUtcTimestamp().Add(_jwtOptions.CustomerTokenLifeSpan);

			var token = new JwtSecurityToken(
				issuer: _jwtOptions.Issuer,
				audience: _jwtOptions.Audience,
				claims: claims,
				expires: expiryDate,
				signingCredentials: creds);

			return new JwtModel(new JwtSecurityTokenHandler().WriteToken(token),
				expiryDate.Ticks);
		}

		/// <inheritdoc />
		public JwtModel CreateSdkToken(string applicationId)
		{
			var provider = new UtcDateTimeProvider();
			var now = provider.GetNow().Add(_jwtOptions.SdkTokenLifeSpan);

			var secondsSinceEpoch = UnixEpoch.GetSecondsSince(now);

			var token = new JwtBuilder()
				.WithSecret(_jwtOptions.SdkSecretKey)
				.WithAlgorithm(new HMACSHA256Algorithm())
				.WithUrlEncoder(new JwtBase64UrlEncoder())
				.WithSerializer(new JsonNetSerializer())
				.WithVerifySignature(true)
				.AddClaim("exp", secondsSinceEpoch)
				.AddClaim("applicationId", applicationId)
				.Encode();

			return new JwtModel(token, secondsSinceEpoch);
		}
	}
}