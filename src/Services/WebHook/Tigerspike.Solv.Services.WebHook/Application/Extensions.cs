using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Core.Email;
using Tigerspike.Solv.Core.Services;

namespace Tigerspike.Solv.Services.WebHook.Application
{
	public static class Extensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddSingleton<ITemplateService>(x => new TemplateService());
			services.AddTransient<ISignatureService, SignatureService>();

			return services;
		}
	}
}