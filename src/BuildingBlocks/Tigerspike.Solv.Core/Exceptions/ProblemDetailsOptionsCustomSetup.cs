using System;
using System.Data;
using System.Net.Http;
using Hellang.Middleware.ProblemDetails;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Mvc;

namespace Tigerspike.Solv.Core.Exceptions
{
	public class ProblemDetailsOptionsCustomSetup : IConfigureOptions<ProblemDetailsOptions>
	{
		private IWebHostEnvironment Environment { get; }
		private IHttpContextAccessor HttpContextAccessor { get; }
		private ApiBehaviorOptions ApiOptions { get; }

		public ProblemDetailsOptionsCustomSetup(IWebHostEnvironment environment,
			IHttpContextAccessor httpContextAccessor, IOptions<ApiBehaviorOptions> apiOptions)
		{
			Environment = environment;
			HttpContextAccessor = httpContextAccessor;
			ApiOptions = apiOptions.Value;
		}

		public void Configure(ProblemDetailsOptions options)
		{
			options.IncludeExceptionDetails = (ctx, ex) =>
				Environment.IsDev() || Environment.IsLocal() || Environment.IsDocker();

			options.OnBeforeWriteDetails = (ctx, details) =>
			{
				// keep consistent with asp.net core 2.2 conventions that adds a tracing value
				ProblemDetailsHelper.SetTraceId(details, HttpContextAccessor.HttpContext);
			};

			// Map known exceptions to 400 instead of default 500.
			options.MapToStatusCode<DomainException>(StatusCodes.Status400BadRequest);
			options.MapToStatusCode<ServiceException>(StatusCodes.Status400BadRequest);

			// This will map HttpRequestException to the 503 Service Unavailable status code.
			options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

			// This will map the MassTransit RequestFaultException to 400
			options.MapToStatusCode<RequestFaultException>(StatusCodes.Status400BadRequest);

			// This will map DBConcurrencyException to the 409 Conflict status code.
			options.MapToStatusCode<DBConcurrencyException>(StatusCodes.Status409Conflict);

			// This will map NotImplementedException to the 501 Not Implemented status code.
			options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

			// Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
			// If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
			options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);

			// You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
			// This is useful if you have upstream middleware that needs to do additional handling of exceptions.
			options.Rethrow<Exception>();
		}
	}
}