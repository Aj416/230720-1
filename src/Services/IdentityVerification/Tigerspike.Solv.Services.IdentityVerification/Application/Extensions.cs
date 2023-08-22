using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Services.IdentityVerification.Application.Services;

namespace Tigerspike.Solv.Services.IdentityVerification.Application
{
	public static class Extensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddTransient<ISignatureService, SignatureService>();
			services.AddScoped<IIdentityVerificationService, OnfidoService>();

			return services;
		}
	}
}