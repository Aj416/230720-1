using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Infra.Bus.Repositories;
using Tigerspike.Solv.Services.Invoicing.Infrastructure.Interfaces;
using Tigerspike.Solv.Services.Invoicing.Infrastructure.Repositories;

namespace Tigerspike.Solv.Services.Invoicing.Infrastructure
{
	public static class Extensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services)
		{
			services.AddScoped<IScheduledJobRepository, ScheduledJobRepository>();
			services.AddScoped<IClientInvoiceRepository, ClientInvoiceRepository>();
			services.AddScoped<IAdvocateInvoiceRepository, AdvocateInvoiceRepository>();
			services.AddScoped<IInvoicingCycleRepository, InvoicingCycleRepository>();
			services.AddScoped<IBillingDetailsRepository, BillingDetailsRepository>();
			services.AddScoped<ISequenceRepository, SequenceRepository>();
			services.AddScoped<IPaymentRepository, PaymentRepository>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			return services;
		}
	}
}
