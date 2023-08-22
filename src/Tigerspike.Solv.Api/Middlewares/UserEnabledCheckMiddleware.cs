using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Tigerspike.Solv.Core.Extensions;
using System.Threading.Tasks;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Core.Services;
using System.Net.Mime;
using System.Net;

namespace Tigerspike.Solv.Api.Middleware
{
	/// <summary>
	/// Middleware for check blocked user
	/// </summary>
	public class UserEnabledCheckMiddleware
	{
		private readonly RequestDelegate _next;

		/// <summary>
		/// Middleware constructor
		/// </summary>
		public UserEnabledCheckMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		/// <summary>
		/// Invoke Method for middleware
		/// </summary>
		public async Task Invoke(HttpContext httpContext, ICachedUserRepository cachedUserRepository, IConfiguredJsonSerializer serializer)
		{
			if (httpContext.User?.Identity?.Name != null)
			{
				var userId = httpContext.User.GetId();
				var userEnabled = await cachedUserRepository.IsUserEnabled(userId);
				if (userEnabled == false)
				{
					// if user is blocked, the Forbid further access
					var content = serializer.Serialize(new { isUserBlocked = true });
					httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
					httpContext.Response.ContentType = MediaTypeNames.Application.Json;
					await httpContext.Response.WriteAsync(content);
					await httpContext.Response.CompleteAsync();
				}
				else
				{
					// if user is not blocked, pass him through
					await _next(httpContext);
				}
			}
			else
			{
				// anonymous requests should be always passed through by this middleware
				await _next(httpContext);
			}
		}

	}

	/// <summary>
	/// Extensions method for middleware calling
	/// </summary>
	public static class UserEnabledExtensions
	{
		/// <summary>
		/// UseUserEnabledCheck middleware
		/// </summary>
		public static IApplicationBuilder UseUserEnabledCheck(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<UserEnabledCheckMiddleware>();
		}
	}
}
