using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.DependencyInjection;

namespace Tigerspike.Solv.Core.Exceptions
{
	public static class Extensions
	{
		public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
		{
			services.ConfigureOptions<ProblemDetailsOptionsCustomSetup>();

			services.AddProblemDetails();

			return services;
		}
	}
}