using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using GreenPipes;
using MassTransit;
using MassTransit.AmazonSqsTransport;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Refit;
using Tigerspike.Solv.Application.CommandHandlers;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Consumers;
using Tigerspike.Solv.Application.Consumers.Chat;
using Tigerspike.Solv.Application.Consumers.IdentityVerification;
using Tigerspike.Solv.Application.Consumers.Invoice;
using Tigerspike.Solv.Application.EventHandlers;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Application.Refit;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Application.Services.Authorization;
using Tigerspike.Solv.Auth0;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Decorators;
using Tigerspike.Solv.Core.Email;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Refit;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Invoice;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Extensions;
using Tigerspike.Solv.Infra.Bus.Repositories;
using Tigerspike.Solv.Infra.Data.Configuration;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Context.Behaviors;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Infra.Data.Repositories;
using Tigerspike.Solv.Infra.Data.Repositories.Cached;
using Tigerspike.Solv.Infra.Storage.Interfaces;
using Tigerspike.Solv.Infra.Storage.S3;
using Tigerspike.Solv.Localization;
using Tigerspike.Solv.Messaging.Invoicing;
using Tigerspike.Solv.Paypal;
using Tigerspike.Solv.Search.CommandHandlers;
using Tigerspike.Solv.Search.EventHandlers;
using Tigerspike.Solv.Search.Implementations;
using Tigerspike.Solv.Search.Interfaces;
using Tigerspike.Solv.Search.Models;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;
using SeedDataExtension = Tigerspike.Solv.Infra.Data.SeedData.SolvDbContextExtensions;

namespace Tigerspike.Solv.Infra.IoC
{
	public class NativeInjectorBootStrapper
	{
		public static void RegisterServices(IConfiguration configuration, IServiceCollection services)
		{
			var retryPolicy = HttpPolicyExtensions
				.HandleTransientHttpError()
				.Or<TimeoutRejectedException>() // Thrown by Polly's TimeoutPolicy if the inner call gets timeout.
				.WaitAndRetryAsync(2, _ => TimeSpan.FromMilliseconds(500));

			var timeoutPolicy = Policy
				.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(5000));

			var invoicingRetryPolicy = HttpPolicyExtensions
				.HandleTransientHttpError()
				.WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

			// ASP.NET HttpContext dependency
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

			// Configuration
			services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
			services.Configure<EmailTemplatesOptions>(configuration.GetSection(EmailTemplatesOptions.SectionName));
			services.Configure<GoogleRecaptchaOptions>(configuration.GetSection(GoogleRecaptchaOptions.SectionName));
			services.Configure<NewProfileOptions>(configuration.GetSection(NewProfileOptions.SectionName));
			services.Configure<Auth0Options>(configuration.GetSection(Auth0Options.SectionName));
			services.Configure<SolvBrandOptions>(configuration.GetSection(SolvBrandOptions.SectionName));
			services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));
			services.Configure<DemoBrandOptions>(configuration.GetSection(DemoBrandOptions.SectionName));
			services.Configure<ServiceUrlsOptions>(configuration.GetSection(ServiceUrlsOptions.SectionName));
			services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
			services.Configure<ElasticSearchOptions>(configuration.GetSection(ElasticSearchOptions.SectionName));
			services.Configure<BusOptions>(configuration.GetSection(BusOptions.SectionName));
			services.Configure<PaypalOptions>(configuration.GetSection(PaypalOptions.SectionName));
			services.Configure<TicketLifecycleOptions>(configuration.GetSection(TicketLifecycleOptions.SectionName));

			// Cache
			services.AddSingleton<ICacheKeyService, CacheKeyService>();

			// AutoMapper
			services.AddAutoMapper(
				typeof(Application.AutoMapper.AutoMapperConfig),
				typeof(Infra.Data.AutoMapper.AutoMapperConfig)
			);

			// Application
			services.AddScoped<IMapper>(
				sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
			services.AddScoped<IBrandService, BrandService>();
			services.AddScoped<IBrandMetadataService, BrandMetadataService>();
			services.AddScoped<IClientService, ClientService>();
			services.AddScoped<IInvoiceService, InvoiceService>();
			services.AddScoped<ITicketService, TicketService>();
			services.AddScoped<ITicketUrlService, TicketUrlService>();
			services.AddScoped<ITicketAutoResponseService, TicketAutoResponseService>();
			services.AddScoped<IAdvocateService, AdvocateService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IPaymentService, PaypalService>();
			services.AddScoped<IAuthenticationService, AuthenticationService>();
			services.AddScoped<ISolvAuthorizationService, SolvAuthorizationService>();
			services.AddScoped<ISearchService<TicketSearchModel>, TicketSearchService>();
			services.AddScoped<ISearchService<AdvocateSearchModel>, AdvocateSearchService>();
			services.AddScoped<ISearchService<AdvocateApplicationSearchModel>, AdvocateApplicationSearchService>();
			services.AddScoped<IWebHookService, WebHookService>();
			services.AddScoped<IQuizService, QuizService>();
			services.AddScoped<IMaintenanceService, MaintenanceService>();
			services.AddScoped<IShortUrlService, ShortUrlService>();
			services.AddScoped<IAdvocateApplicationService, AdvocateApplicationService>();
			services.AddScoped<ICustomerService, CustomerService>();
			services.AddTransient<IUploaderService, S3UploaderService>();

			// External Services
			services.AddRefitClient<IChatServiceClient>(RefitExtensions.GetNewtonsoftJsonRefitSettings())
				.ConfigureHttpClient(c => c.BaseAddress = new Uri(configuration.GetSection("ServiceUrls")["Chat"]))
				.AddPolicyHandler(retryPolicy)
				.AddPolicyHandler(timeoutPolicy);

			services.Decorate<IChatServiceClient, ChatService>();
			services.AddScoped<IChatService, ChatService>();

			services.AddRefitClient<IInvoicingServiceClient>()
				.ConfigureHttpClient(c => c.BaseAddress = new Uri(configuration.GetSection("ServiceUrls")["Invoicing"]))
				.AddPolicyHandler(invoicingRetryPolicy);

			services.Decorate<IInvoicingServiceClient, InvoicingService>();
			services.AddScoped<IInvoicingService, InvoicingService>();

			services.AddRefitClient<IIdentityVerificationClient>(RefitExtensions.GetNewtonsoftJsonRefitSettings())
				.ConfigureHttpClient(c => c.BaseAddress = new Uri(configuration.GetSection("ServiceUrls")["IdentityVerification"]))
				.AddPolicyHandler(retryPolicy)
				.AddPolicyHandler(timeoutPolicy);

			services.AddScoped<IIdentityVerificationService, IdentityVerificationService>();

			// Domain Notification Handler
			services.AddScoped<IDomainNotificationHandler, DomainNotificationHandler>();
			services.AddScoped<INotificationHandler<DomainNotification>>(
				x => x.GetService<IDomainNotificationHandler>());

			// all notification handlers
			services.Scan(scan => scan
				.FromAssembliesOf(typeof(IdentityEventHandler),
					typeof(TicketSearchEventHandler))
				.AddClasses(x => x.AssignableTo(typeof(INotificationHandler<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime()
			);

			// decorators for MediatR notification handlers
			services.Decorate(typeof(INotificationHandler<>), typeof(NotificationLoggerDecorator<>));

			// all request handlers
			services.Scan(scan => scan
				.FromAssembliesOf(typeof(UserCommandHandler))
				.AddClasses(x => x.AssignableTo(typeof(IRequestHandler<,>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime()
			);

			services.Scan(scan => scan
				.FromAssembliesOf(typeof(UserCommandHandler))
				.AddClasses(x => x.AssignableTo(typeof(AsyncRequestHandler<>)))
				.AsSelfWithInterfaces()
				.WithScopedLifetime()
			);

			services.Scan(scan => scan
				.FromAssembliesOf(typeof(TicketSearchCommandHandler))
				.AddClasses(x => x.AssignableTo(typeof(IRequestHandler<,>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime()
			);

			// Infra - Data
			services.AddScoped<IEventStore, InMemoryEventStore>();
			services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
			services.AddScoped<IBillingDetailsRepository, BillingDetailsRepository>();
			services.AddScoped<IAdvocateApplicationRepository, AdvocateApplicationRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IAdvocateRepository, AdvocateRepository>();
			services.AddScoped<IClientRepository, ClientRepository>();
			services.AddScoped<IAdvocateBrandRepository, AdvocateBrandRepository>();
			services.AddScoped<ITicketEscalationConfigRepository, TicketEscalationConfigRepository>();
			services.AddScoped<ITicketRepository, TicketRepository>();
			services.AddScoped<ITicketImportRepository, TicketImportRepository>();
			services.AddScoped<ITrackingDetailRepository, TrackingDetailRepository>();
			services.AddScoped<ITicketSourceRepository, TicketSourceRepository>();
			services.AddScoped<ITicketImportRepository, TicketImportRepository>();
			services.AddScoped<IAreaRepository, AreaRepository>();
			services.AddScoped<IAdvocateApplicationBrandRepository, AdvocateApplicationBrandRepository>();
			services.AddScoped<IApplicationAnswerRepository, ApplicationAnswerRepository>();
			services.AddScoped<IAnswerRepository, AnswerRepository>();
			services.AddScoped<IBrandRepository, BrandRepository>();
			services.AddScoped<IBrandMetadataAccessRepository, BrandMetadataAccessRepository>();
			services.AddScoped<IBrandMetadataRoutingConfigRepository, BrandMetadataRoutingConfigRepository>();
			services.AddScoped<IBrandRepository, BrandRepository>();
			services.AddScoped<IBrandAdvocateResponseConfigRepository, BrandAdvocateResponseConfigRepository>();
			services.AddScoped<IBrandTicketPriceHistoryRepository, BrandTicketPriceHistoryRepository>();
			services.AddScoped<IQuestionRepository, QuestionRepository>();
			services.AddScoped<IProfileBrandRepository, ProfileBrandRepository>();
			services.AddScoped<IClientInvoiceRepository, ClientInvoiceRepository>();
			services.AddScoped<IAdvocateInvoiceRepository, AdvocateInvoiceRepository>();
			services.AddScoped<IInvoicingCycleRepository, InvoicingCycleRepository>();
			services.AddScoped<IPaymentRepository, PaymentRepository>();
			services.AddScoped<IQuizRepository, QuizRepository>();
			services.AddScoped<ISequenceRepository, SequenceRepository>();
			services.AddScoped<IQuizAdvocateAttemptRepository, QuizAdvocateAttemptRepository>();
			services.AddScoped<ITicketTagRepository, TicketTagRepository>();

			// Cached repositories
			services.AddScoped<ICachedBrandRepository, CachedBrandRepository>();
			services.AddScoped<ICachedTicketRepository, CachedTicketRepository>();
			services.AddScoped<ICachedAdvocateRepository, CachedAdvocateRepository>();
			services.AddScoped<ICachedUserRepository, CachedUserRepository>();
			services.AddScoped<ICachedBrandAdvocateResponseConfigRepository, CachedBrandAdvocateResponseConfigRepository>();

			// Dynamodb repositories
			services.AddDynamoDb(configuration.GetSection(DynamoDbOptions.SectionName));
			services.AddScoped<IScheduledJobRepository, ScheduledJobRepository>();

			// Other infrastructure services
			services.AddSingleton<ITemplateService>(x => new TemplateService());
			services.AddScoped<IJwtService, JwtService>();
			services.AddTransient<ISignatureService, SignatureService>();
			services.AddTransient<ISignedUrlGenerator, S3PreSignedUrlGenerator>();

			// serialization
			services.AddTransient<JsonSerializerSettings>(x => new JsonSerializerSettings().AsDefault());
			services.AddTransient<IConfiguredJsonSerializer, ConfiguredJsonSerializer>();
			services.AddTransient<IJsonContentProvider, JsonContentProvider>();
			services.AddTransient<ICsvSerializer, CsvSerializer>();

			// Database context
			services.AddScoped<DbContext, SolvDbContext>();
			services.AddDbContext<SolvDbContext>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IContextBehavior, CreatedDateBehavior>();
			services.AddScoped<IContextBehavior, ModifiedDateBehavior>();

			// Identity
			services.Configure<InvoicingOptions>(configuration.GetSection(InvoicingOptions.SectionName));

			// Helper services
			services.AddScoped<ITimestampService, TimestampService>();

			// Localization
			services.AddSingleton<ILocalizationProviderFactory>(x => new LocalizationProviderFactory());

			services.AddSingleton<INewProfileService, NewProfileService>();

			// Inject a HttpClient with automatic lifecycle management
			services.AddHttpClient<IRecaptchaApiClient, RecaptchaApiClient>(client =>
			{
				// Sets the HttpClient timeout value to the GoogleRecaptchaOptions.Timeout setting,
				// or 30 seconds if the setting cannot be found.
				client.Timeout = TimeSpan.FromSeconds(configuration.GetSection(GoogleRecaptchaOptions.SectionName)
					.GetValue(nameof(GoogleRecaptchaOptions.Timeout), 30));
			});

			services.AddBus(x =>
			{
				x.AddMessageScheduler();

				x.AddConsumersFromNamespaceContaining<TicketConsumer>();
				x.AddConsumersFromNamespaceContaining<EmailConsumer>();
				x.AddConsumersFromNamespaceContaining<UpdateSearchIndexWhenChatMessageAddedEventConsumer>();
				x.AddConsumersFromNamespaceContaining<NotifyWhenChatEventsConsumer>();

				x.AddBus(provider => provider.ConfigureTransport(GetEndpoints(provider)));

				x.AddRequestClient<StartInvoicingCycleCommand>();
			});

		}

		private static List<BusEndpointConfiguration> GetEndpoints(IServiceProvider provider)
		{
			var options = provider.GetRequiredService<IOptions<BusOptions>>().Value;
			return new List<BusEndpointConfiguration>
			{
				new BusEndpointConfiguration
				{
					QueueName = options.Queues.Schedule,
					EndpointConfigurator = c =>
					{
						c.Consumer<RecurringInvoicingCycleConsumer>(provider);
						EndpointConvention.Map<RecurringInvoicingCycleCommand>(c.InputAddress);
					}

				},
				new BusEndpointConfiguration
				{
					QueueName = options.Queues.Advocate,
					EndpointConfigurator = c =>
					{
						c.Consumer<AdvocateApplicationConsumer>(provider);
						c.Consumer<AdvocateConsumer>(provider);
						c.Consumer<IdentityCheckCreatedEventConsumer>(provider);
						c.Consumer<IdentityCheckCompletedEventConsumer>(provider);
						c.Consumer<IdentityCreatedEventConsumer>(provider);

						EndpointConvention.Map<SendProfilingReminderCommand>(c.InputAddress);
						EndpointConvention.Map<IFetchAdvocateIdsForInvoicingCommand>(c.InputAddress);
						EndpointConvention.Map<IFetchBrandIdsForInvoicingCommand>(c.InputAddress);
						EndpointConvention.Map<IFetchAdvocateInfoCommand>(c.InputAddress);
						EndpointConvention.Map<IFetchAdvocateDetailsCommand>(c.InputAddress);
					}
				},
				new BusEndpointConfiguration
				{
					QueueName = options.Queues.Ticket,
					EndpointConfigurator = c =>
					{
						c.Consumer<TicketConsumer>(provider);

						// Fraud consumers
						c.Consumer<FraudDetectionCompletedEventConsumer>(provider);

						// Chat consumers
						c.Consumer<NotifyWhenChatEventsConsumer>(provider);
						c.Consumer<NotifyWhenEmailTransportConsumer>(provider);
						c.Consumer<ReturningCustomerConsumer>(provider);
						c.Consumer<ScheduledReminderWhenChatMessageAddedEventConsumer>(provider);
						c.Consumer<TicketNotificationResumptionConsumer>(provider);
						c.Consumer<UpdateTicketStatisticsWhenChatMessageAddedConsumer>(provider);
						c.Consumer<SendFirstAdvocateResponseConsumer>(provider);
						c.Consumer<UpdateSearchIndexWhenChatMessageAddedEventConsumer>(provider);
						c.Consumer<ChatActionFinalizedEventConsumer>(provider);
						c.Consumer<ChatAutoResponseCompletedEventConsumer>(provider);

						EndpointConvention.Map<CancelTicketReservationCommand>(c.InputAddress);
						EndpointConvention.Map<CloseTicketWhenNoResponseCommand>(c.InputAddress);
						EndpointConvention.Map<StartTicketTimeoutEscalation>(c.InputAddress);
						EndpointConvention.Map<SendChatReminderCommand>(c.InputAddress);
						EndpointConvention.Map<SendCloseTicketReminderCommand>(c.InputAddress);
						EndpointConvention.Map<DelayChatResponseCommand>(c.InputAddress);
						EndpointConvention.Map<IFetchClientInvoicingAmountCommand>(c.InputAddress);
						EndpointConvention.Map<IFetchAdvocatesToInvoiceCommand>(c.InputAddress);
						EndpointConvention.Map<IFetchTicketsForAdvocateInvoiceCommand>(c.InputAddress);
						EndpointConvention.Map<IFetchTicketInfoCommand>(c.InputAddress);
						EndpointConvention.Map<IFetchTicketsInfoForAdvocateInvoiceCommand>(c.InputAddress);
					}
				},
				new BusEndpointConfiguration
				{
					QueueName = options.Queues.Import,
					EndpointConfigurator = c =>
					{
						c.Consumer<ImportConsumer>(provider);
						EndpointConvention.Map<ImportTicketCommand>(c.InputAddress);

						c.UseConcurrencyLimit(1); // do not process import items in parallel
						c.UseRateLimit(100, TimeSpan.FromMinutes(1)); // limit number of items to be processed in a period of time, so the we would not overwhelm ourselves

						if (c is IAmazonSqsReceiveEndpointConfigurator configurator)
						{
							configurator.ConfigureConsumeTopology = false;
							configurator.PrefetchCount = 1; // take one message at a time from SQS
						}
					}
				},
				new BusEndpointConfiguration
				{
					QueueName = options.Queues.Export,
					EndpointConfigurator = c =>
					{
						if (c is MassTransit.AmazonSqsTransport.IQueueConfigurator sqsConfigurator)
						{
							// Amazon SQS specific configuration to allow messages to be consumed for longer period than default 30 seconds
							sqsConfigurator.QueueAttributes["VisibilityTimeout"] = TimeSpan.FromMinutes(15).TotalSeconds;
						}

						if (c is IAmazonSqsReceiveEndpointConfigurator configurator)
						{
							configurator.PrefetchCount = 1; // take one message at a time from SQS
						}

						c.Consumer<ExportConsumer>(provider);
						EndpointConvention.Map<CreateTicketsExportCommand>(c.InputAddress);

						c.UseConcurrencyLimit(1); // do not process export items in parallel
					}
				},
				new BusEndpointConfiguration
				{
					QueueName = options.Queues.Invoicing,
					EndpointConfigurator = c =>
					{
						// temporarly disable retries on that queue, as they behave unpredictably on SIT/UAT (retry affects also properly delivered messages)
						// c.UseMessageRetry(x => x.Incremental(5, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(15)));

						c.Consumer<StartInvoicingCycleConsumer>(provider);
						c.Consumer<GenerateInvoicingCycleInvoicesConsumer>(provider);
						c.Consumer<CreateClientInvoiceConsumer>(provider);
						c.Consumer<CreateAdvocateInvoiceConsumer>(provider);
						EndpointConvention.Map<StartInvoicingCycleCommand>(c.InputAddress);
						EndpointConvention.Map<GenerateInvoicingCycleInvoices>(c.InputAddress);
						EndpointConvention.Map<CreateClientInvoiceCommand>(c.InputAddress);
						EndpointConvention.Map<CreateAdvocateInvoiceCommand>(c.InputAddress);
					}
				},
				new BusEndpointConfiguration
				{
					QueueName = options.Queues.Email,
					EndpointConfigurator = c =>
					{
						c.Consumer<EmailConsumer>(provider);
						EndpointConvention.Map<ReceiveEmailMessageCommand>(c.InputAddress);
					}
				},
				new BusEndpointConfiguration
				{
					QueueName = options.Queues.Brand,
					EndpointConfigurator = c =>
					{
						c.Consumer<BrandConsumer>(provider);
						EndpointConvention.Map<IBrandInfoCommand>(c.InputAddress);
						EndpointConvention.Map<ISetBillingDetailsIdCommand>(c.InputAddress);
						EndpointConvention.Map<IBrandIdForInvoicingCommand>(c.InputAddress);
						EndpointConvention.Map<IFetchBrandBillingDetailCommand>(c.InputAddress);
					}
				},
				new BusEndpointConfiguration
				{
					QueueName = options.Queues.Payment,
					EndpointConfigurator = c =>
					{
						c.Consumer<PaymentConsumer>(provider);
						EndpointConvention.Map<IRiskTransactionContextCommand>(c.InputAddress);
						EndpointConvention.Map<IExecutePaymentCommand>(c.InputAddress);
						EndpointConvention.Map<IFetchPaymentReceiverAccountIdCommand>(c.InputAddress);
					}
				},
			};
		}

		public static async Task MigrateDatabase(IServiceScopeFactory serviceScopeFactory)
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var logger = scope.ServiceProvider.GetRequiredService<ILogger<NativeInjectorBootStrapper>>();

				var database = scope.ServiceProvider.GetService<SolvDbContext>().Database;
				logger.LogInformation($"Running system migrations for database {database.GetDbConnection().Database}");
				await database.MigrateAsync();
			}
		}

		public static async Task EnsureSeedData(IServiceScopeFactory serviceScopeFactory)
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var solvBrandOptions = scope.ServiceProvider.GetService<IOptions<SolvBrandOptions>>();
				var demoBrandOptions = scope.ServiceProvider.GetService<IOptions<DemoBrandOptions>>();

				await SeedDataExtension.EnsureSeedData(scope.ServiceProvider.GetService<SolvDbContext>(),
					solvBrandOptions.Value, demoBrandOptions.Value);
			}
		}
	}
}