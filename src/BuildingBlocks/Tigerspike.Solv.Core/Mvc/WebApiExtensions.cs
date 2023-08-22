using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.FeatureManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Tigerspike.Solv.Core.Mvc
{
	public static class WebApiExtensions
	{
		public static IServiceCollection AddWebApi(this IServiceCollection services,
			Action<FluentValidationMvcConfiguration> fluentValidationAction)
		{
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services
				.AddRouting(options => options.LowercaseUrls = true)
				.AddMvcCore()
				.AddDefaultJsonOptions()
				.AddDataAnnotations()
				.AddApiExplorer()
				.AddAuthorization();

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
				options.UseApiBehavior = false;
			});

			services.AddControllers(options =>
				{
					options.OutputFormatters.Remove(new XmlDataContractSerializerOutputFormatter());
				}).AddFluentValidation(fluentValidationAction)
				.AddNewtonsoftJson(options => options.SerializerSettings.AsDefault());

			services.AddHealthChecks();

			services.AddFeatureManagement();

			return services;
		}

		private static IMvcCoreBuilder AddDefaultJsonOptions(this IMvcCoreBuilder builder)
			=> builder.AddNewtonsoftJson(o => o.SerializerSettings.AsDefault());

		public static IApplicationBuilder UseWebApiEndpoints(this IApplicationBuilder app,
			Action<IEndpointRouteBuilder> build,
			bool useAuthorization = false, bool useAuthentication = false,
			Action<IApplicationBuilder> middleware = null)
		{
			app.UseRouting();

			if (useAuthorization)
			{
				app.UseAuthorization();
			}

			if (useAuthentication)
			{
				app.UseAuthentication();
			}

			middleware?.Invoke(app);

			app.UseHealthChecks();

			app.UseEndpoints(build);

			return app;
		}

		public static IEndpointRouteBuilder MapHealthChecks(this IEndpointRouteBuilder routeBuilder,
			string path = "/healthcheck")
		{
			routeBuilder.MapHealthChecks("/healthcheck", GetDefaultHealthCheckOptions());

			return routeBuilder;
		}

		public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app, string path = "/healthcheck")
		{
			app.UseHealthChecks("/healthcheck", GetDefaultHealthCheckOptions());

			return app;
		}

		public static IEndpointRouteBuilder UseDefaults(this IEndpointRouteBuilder builder)
		{
			builder.MapControllers();
			builder.MapHealthChecks();

			return builder;
		}

		private static HealthCheckOptions GetDefaultHealthCheckOptions()
		{
			var options = new HealthCheckOptions();
			options.ResultStatusCodes[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable;
			options.Predicate = check => check.Tags.Contains("ready");

			options.ResponseWriter = HealthCheckWriteResponse;

			return options;
		}

		private static Task HealthCheckWriteResponse(HttpContext httpContext, HealthReport result)
		{
			httpContext.Response.ContentType = "application/json";

			var json = new JObject(
				new JProperty("status", result.Status.ToString()),
				new JProperty("results", new JObject(result.Entries.Select(pair =>
					new JProperty(pair.Key, new JObject(
						new JProperty("status", pair.Value.Status.ToString()),
						new JProperty("description", pair.Value.Description)
					))))));

			return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
		}

		public static JsonSerializerSettings AsDefault(this JsonSerializerSettings settings)
		{
			settings.ContractResolver = new CamelCasePropertyNamesContractResolver
			{
				NamingStrategy = {ProcessDictionaryKeys = false}
			};

			settings.Converters.Add(new StringEnumConverter()); // serialize enums as strings

			settings.NullValueHandling = NullValueHandling.Ignore; // do not serialize null values
			settings.DefaultValueHandling =
				DefaultValueHandling
					.Include; // always include default values (especially needed when sending enums to the outside world, e.g. in webhooks)
			settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc; // always deal with dates in UTC manner
			settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
			settings.DateParseHandling = DateParseHandling.DateTime;
			settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
			settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			settings.Formatting = Formatting.Indented;

			return settings;
		}
	}
}