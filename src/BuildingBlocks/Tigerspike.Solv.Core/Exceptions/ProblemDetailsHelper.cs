using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tigerspike.Solv.Core.Exceptions
{
	public static class ProblemDetailsHelper
	{
		public static void SetTraceId(ProblemDetails details, HttpContext httpContext)
		{
			// this is the same behaviour that Asp.Net core uses
			var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
			details.Extensions["traceId"] = traceId;
		}
	}
}