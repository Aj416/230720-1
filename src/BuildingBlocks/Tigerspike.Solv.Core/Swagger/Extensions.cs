using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Core.Swagger
{
    public static class Extensions
    {
        public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
        {
            SwaggerOptions options;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                services.Configure<SwaggerOptions>(configuration.GetSection("Swagger"));
                options = configuration.GetOptions<SwaggerOptions>("Swagger");
            }

            if (!options.Enabled)
            {
                return services;
            }

            services.AddSwaggerGen(c =>
            {
	            c.SwaggerDoc(options.Name, new OpenApiInfo {Title = options.Title, Version = options.Version});

				if (options.IncludeSecurity)
				{
					var openApiSecurityScheme = new OpenApiSecurityScheme
					{
						Type = SecuritySchemeType.OAuth2,
						Flows = new OpenApiOAuthFlows
						{
							Implicit = new OpenApiOAuthFlow
							{
								AuthorizationUrl =
									new Uri(options.AuthorizationUrl),
								Scopes = new Dictionary<string, string> { { "openid", "Standard OpenId" } }
							}
						},
						Scheme = "oauth2"
					};

					c.AddSecurityDefinition("oauth2", openApiSecurityScheme);

					c.AddSecurityRequirement(new OpenApiSecurityRequirement
					{
						{
							openApiSecurityScheme,
							new[] {"openid"}
						}
					});
				}

				// Use method name as operationId
				c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo)
					? methodInfo.Name.ToCamelCase()
					: null);

				c.UseInlineDefinitionsForEnums();

				// add swagger filters
				c.OperationFilter<SecurityRequirementsOperationFilter>();
				c.OperationFilter<FeatureToggleFilter>();
				c.DocumentFilter<DisabledOperationFilter>();
            });

            // opt in to Newtonsoft for Swashbuckle
            return
                services.AddSwaggerGenNewtonsoftSupport();
        }

        public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder builder)
        {
            var options = builder.ApplicationServices.GetService<IConfiguration>()
                .GetOptions<SwaggerOptions>("Swagger");
            if (!options.Enabled)
            {
                return builder;
            }

            var routePrefix = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "swagger" : options.RoutePrefix;

            builder.UseStaticFiles()
                .UseSwagger(c => c.RouteTemplate = routePrefix + "/{documentName}/swagger.json");

            return builder.UseSwaggerUI(c =>
            {
	            c.SwaggerEndpoint($"/{routePrefix}/{options.Name}/swagger.json", options.Title);
	            c.RoutePrefix = routePrefix;

	            c.DisplayRequestDuration();

	            c.DisplayOperationId();

	            if (options.IncludeSecurity)
	            {
		            c.OAuthClientId(options.OAuthClientId);
		            c.OAuthAppName("Swagger UI");
	            }
            });
        }
    }
}