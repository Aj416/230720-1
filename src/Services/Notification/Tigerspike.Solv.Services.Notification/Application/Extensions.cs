using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Services.Notification.Application.Services;
using Tigerspike.Solv.Localization;
using Tigerspike.Solv.Core.Email;
using Tigerspike.Solv.Localization.Json.ResourcesModel;

namespace Tigerspike.Solv.Services.Notification.Application
{
	public static class Extensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddTransient<IEmailService, EmailService>();
			services.AddScoped<IMessengerService, MessengerService>();
			services.AddSingleton<ITemplateService>(x => new TemplateService());
			services.AddSingleton<ILocalizationProviderFactory>(x => new LocalizationProviderFactory());

			LiquidStaticInitializer.RegisterAssemblyWith<ResourcesModel>();

			return services;
		}
	}
}