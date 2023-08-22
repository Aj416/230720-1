using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Tigerspike.Solv.Api.Authentication.ApiKey;
using Tigerspike.Solv.Api.Authentication.Sdk;
using Tigerspike.Solv.Api.Configuration;
using Tigerspike.Solv.Api.Middleware;
using Tigerspike.Solv.Api.Services;
using Tigerspike.Solv.Application.Extensions;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Application.SignalR;
using Tigerspike.Solv.Auth0;
using Tigerspike.Solv.Chat.SignalR;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Exceptions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.RateLimiting;
using Tigerspike.Solv.Core.Redis;
using Tigerspike.Solv.Core.ServiceStack;
using Tigerspike.Solv.Core.Swagger;
using Tigerspike.Solv.Infra.IoC;
using Tigerspike.Solv.Infra.IoC.Host;

namespace Tigerspike.Solv.Api
{
	/// <summary>
	/// Startup Class
	/// </summary>
	public class Startup
	{
		private const string CHAT_HUB_URL = "/hubs/chat";
		private const string TICKET_HUB_URL = "/hubs/ticket";

		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _hostingEnvironment;

		public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
		{
			_configuration = configuration;
			_hostingEnvironment = hostEnvironment;
		}

		/// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// For more information on how to configure your application, visit
		/// https://go.microsoft.com/fwlink/?LinkID=398940
		/// </summary>
		/// <param name="services"></param>
		public void ConfigureServices(IServiceCollection services)
		{
			var knownNetworks = _configuration.GetSection("ForwardHeaderOptions").GetSection("KnownNetwork").Get<List<string>>() ?? new List<string>();
			services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.ForwardedHeaders =
						ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
				foreach (var knownNetwork in knownNetworks)
				{
					options.KnownNetworks.Add(new IPNetwork(
						IPAddress.Parse(knownNetwork), 104));
				}
			});

			services.AddRateLimiting();

			services.AddHostedService<MigratorHostedService>();

			AddAuthentication(services);

			// Add framework services.

			services.AddControllers(options =>
				{
					options.OutputFormatters.Remove(new XmlDataContractSerializerOutputFormatter());
				})
				.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateTicketModelValidator>())
				.AddNewtonsoftJson(options => options.SerializerSettings.AsDefault());

			services.AddRouting(options => options.LowercaseUrls = true);

			services.AddHealthChecks();

			// add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
			// note: the specified format code will format the version as "'v'major[.minor][-status]"
			services.AddVersionedApiExplorer(options =>
			{
				options.GroupNameFormat = "'v'VVV";
				options.SubstituteApiVersionInUrl = true;
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
			});

			services.AddApiVersioning(options =>
			{
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.ReportApiVersions = true;
			});

			// Add service and create Policy with options
			services.AddCors(options =>
			{
				var corsConfig = _configuration.GetSection(CorsConfiguration.CorsSection);
				options.AddDefaultPolicy(
					builder => builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader()
					.WithExposedHeaders("Location"));

				options.AddPolicy(CorsConfiguration.MarketingSitePolicy,
					builder =>
					{
						var policyConfig = corsConfig.GetSection(CorsConfiguration.MarketingSitePolicy);
						var origins = policyConfig.GetSection(CorsConfiguration.OriginsSection)
							.AsEnumerable()
							.Select(x => x.Value)
							.Where(x => x != null)
							.ToArray();

						builder
							.SetIsOriginAllowedToAllowWildcardSubdomains()
							.WithOrigins(origins)
							.AllowAnyHeader()
							.AllowAnyMethod();
					});
			});

			services.AddAuthorization();

			services.AddAuthentication();

			services.AddFeatureManagement();

			services.AddSwaggerDocs();

			// add reference to redis cache store
			services.AddRedisCache(_configuration.GetSection(RedisOptions.SectionName));

			// Adding MediatR for Domain Events and Notifications
			services.AddMediator(typeof(Startup));

			// AWS
			services.AddDefaultAWSOptions(_configuration.GetAWSOptions());
			services.AddAWSService<IAmazonS3>();

			// SignalR
			services.AddSignalR()
				.AddNewtonsoftJsonProtocol(options => options.PayloadSerializerSettings.AsDefault())
				.AddRedis(options =>
				{
					var redisOptions = _configuration.GetOptions<RedisOptions>(RedisOptions.SectionName);

					foreach (var endpoint in redisOptions.GetReadWriteEndpoints())
					{
						options.Configuration.EndPoints.Add(endpoint);
					}

					options.Configuration.Ssl = _configuration.GetSection(RedisOptions.SectionName)
						.GetValue<bool>(nameof(RedisOptions.Ssl));
					options.Configuration.ChannelPrefix = _hostingEnvironment.EnvironmentName;
				});

			services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
			services.AddScoped<IFraudService, FraudService>();

			services.AddExceptionHandling();

			services.AddSolvWorkflow(_configuration);

			// .NET Native DI Abstraction
			RegisterServices(_configuration, services);
		}

		/// <summary>
		/// This method gets called by the runtime. Use this method to configure the HTTP request
		/// pipeline.
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		/// <param name="provider"></param>
		public void Configure(IApplicationBuilder app,
			IWebHostEnvironment env,
			IApiVersionDescriptionProvider provider)
		{

			if (env.IsDev() || env.IsLocal() || env.IsDocker())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseForwardedHeaders();

			app.UseServiceStack();

			app.UseSolvWorkflows();

			// Use problem details exception handler.
			app.UseProblemDetails();

			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

			app.UseRouting();
			app.UseCors();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseUserEnabledCheck();

			app.UseRateLimiting();
			app.UseHealthChecks();

			app.UseEndpoints(endpoints =>
			{
				endpoints.UseDefaults();
				endpoints.MapHub<ChatHub>(CHAT_HUB_URL);
				endpoints.MapHub<TicketHub>(TICKET_HUB_URL);
			});

			app.UseStaticFiles();

			app.UseSwaggerDocs();

			// add security headers
			app.UseSecurityHeaders(policies =>
				policies
				.AddDefaultSecurityHeaders()
				.AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 63072000)
			);
		}

		private static void RegisterServices(IConfiguration configuration, IServiceCollection services) =>
			// Adding dependencies from another layers (isolated from Presentation)
			NativeInjectorBootStrapper.RegisterServices(configuration, services);

		private void AddAuthentication(IServiceCollection services)
		{
			var jwtOptions = _configuration.GetOptions<JwtOptions>(JwtOptions.SectionName);
			var auth0Options = _configuration.GetOptions<Auth0Options>(Auth0Options.SectionName);

			if (jwtOptions == null)
			{
				return;
			}

			services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddApiKeyAuthentication(options => { })
				.AddSdkAuthentication(options => { })
				.AddJwtBearer(options =>
				{
					var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

					options.Authority = jwtOptions.Authority;
					options.Audience = jwtOptions.Audience;

					options.TokenValidationParameters = new TokenValidationParameters
					{
						// OpenID standard claim name for user id (aka identity name) is sub.
						NameClaimType = "sub",
						ValidIssuers = new[]
						{
							jwtOptions.Issuer,
							auth0Options.ManagementApi.Authority
						},
						ValidAudiences = jwtOptions.ValidAudiences,
						IssuerSigningKey = key
					};

					// We have to hook the OnMessageReceived event in order to allow the JWT
					// authentication handler to read the access token from the query string when a
					// WebSocket or Server-Sent Events request comes in.
					options.Events = new JwtBearerEvents
					{
						OnTokenValidated = context =>
							{
								if (context.SecurityToken is JwtSecurityToken token &&
									context.Principal.Identity is ClaimsIdentity identity)
								{
									identity.AddClaim(new Claim("access_token", token.RawData));
								}

								return Task.FromResult(0);
							},
						OnMessageReceived = context =>
						{
							var accessToken = context.Request.Query["access_token"];

							// If the request is for our hub...
							var path = context.HttpContext.Request.Path;
							if (!string.IsNullOrEmpty(accessToken) &&
								(path.StartsWithSegments(CHAT_HUB_URL) || path.StartsWithSegments(TICKET_HUB_URL)))
							{
								// Read the token out of the query string
								context.Token = accessToken;
							}

							return Task.CompletedTask;
						}
					};
				});
		}
	}
}