using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tigerspike.Solv.Application.Configuration;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Application.Workflows;
using Tigerspike.Solv.Application.Workflows.Steps;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.DynamoDb;
using Tigerspike.Solv.Infra.Bus.Configuration;
using WorkflowCore.Interface;

namespace Tigerspike.Solv.Application.Extensions
{
	/// <summary>
	/// The workflow extensions
	/// </summary>
	public static class WorkflowExtensions
	{
		/// <summary>
		/// The helper method to add workflows
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configuration"></param>
		public static void AddSolvWorkflow(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<DynamoDbOptions>(configuration.GetSection(DynamoDbOptions.SectionName));
			services.Configure<BusOptions>(configuration.GetSection(BusOptions.SectionName));
			
			var dynamoOptions = configuration.GetOptions<DynamoDbOptions>(DynamoDbOptions.SectionName);
			var busOptions = configuration.GetOptions<BusOptions>(BusOptions.SectionName);

			services.AddWorkflow(cfg => cfg.Setup(dynamoOptions, busOptions));
			services.AddWorkflowSteps();
		}

		/// <summary>
		/// The helper method to use the workflows
		/// </summary>
		/// <param name="app"></param>
		public static void UseSolvWorkflows(this IApplicationBuilder app)
		{
			var serviceProvider = app.ApplicationServices;
			var host = serviceProvider.GetService<IWorkflowHost>();
			host.RegisterWorkflows();
			host.Start();
		}

		/// <summary>
		/// Method to register all workflows
		/// </summary>
		/// <param name="host"></param>
		private static void RegisterWorkflows(this IWorkflowHost host)
		{
			host.RegisterWorkflow<CreateTicketWorkflow, CreateTicketWorkflowModel>();
			host.RegisterWorkflow<CompleteTicketWorkflow, CompleteTicketWorkflowModel>();
		} 

		/// <summary>
		/// Method to define scopes of all the workflow steps
		/// </summary>
		/// <param name="services"></param>
		private static void AddWorkflowSteps(this IServiceCollection services)
		{
			services.AddTransient<CreateTicketInitiate>();
			services.AddTransient<CreateTicketFinalize>();
			services.AddTransient<NotifyAdvocateStatisticsUpdated>();
			services.AddTransient<NotifyAvailableTicketsChanged>();
			services.AddTransient<NotifyTicketCreation>();
			services.AddTransient<NotifyTicketStatusChanged>();
			services.AddTransient<SendTicketSystemMessages>();
			services.AddTransient<UpdateAdvocateStatistics>();
			services.AddTransient<UpdateTicketSearchService>();
		}
	}
}
