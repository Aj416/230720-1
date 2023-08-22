using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tigerspike.Solv.Core.Swagger
{
	/// <summary>
	/// Security filter for apis requiring authorization.
	/// </summary>
	public class SecurityRequirementsOperationFilter : IOperationFilter
	{
		/// <summary>
		/// The filter apply method.
		/// </summary>
		/// <param name="operation">The operation.</param>
		/// <param name="context">The context.</param>
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			// Policy names map to scopes
			var requiredScopes = context.MethodInfo
				.GetCustomAttributes(true)
				.OfType<AuthorizeAttribute>()
				.Union(context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>())
				.Select(attr => attr.Policy)
				.Distinct()
				.ToList();

			if (requiredScopes.Any())
			{
				var oAuthScheme = new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = "oauth2"
					}
				};

				operation.Security = new List<OpenApiSecurityRequirement>
				{
					new OpenApiSecurityRequirement
					{
						[oAuthScheme] = new List<string> { "openid" }
					}
				};
			}
		}
	}
}