using System;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Core.Configuration;

namespace Tigerspike.Solv.Services.Notification.Configuration
{
	public static class Extensions
	{
		public static IServiceCollection AddConfiguration(this IServiceCollection services)
		{
			using (var serviceProvider = services.BuildServiceProvider())
			{
				var configuration = serviceProvider.GetService<IConfiguration>();
				services.Configure<MessengerOptions>(configuration.GetSection(MessengerOptions.SectionName));
			}

			return services;
		}

		public static void AddEmailProvider(this IServiceCollection services)
		{
			EmailOptions options;
			using (var serviceProvider = services.BuildServiceProvider())
			{
				var configuration = serviceProvider.GetService<IConfiguration>();
				services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
				options = configuration.GetOptions<EmailOptions>(EmailOptions.SectionName);
			}

			services
				.AddFluentEmail(options.DefaultEmail)
				.AddSmtpSender(() => new SmtpClient
				{
					Host = options.Host,
					Port = Convert.ToInt32(options.Port),
					EnableSsl = true,
					Credentials = new System.Net.NetworkCredential(
						options.Username,
						options.Password)
				});
		}
	}
}