using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.HealthChecks;

namespace Tigerspike.Solv.Infra.Bus
{
	public class BusHostedService :
		IHostedService
	{
		private readonly IBusControl _busControl;
		private readonly ILogger<BusHostedService> _logger;
		private readonly SimplifiedBusHealthCheck _simplifiedBusCheck;
		private readonly ReceiveEndpointHealthCheck _receiveEndpointCheck;
		private readonly BusOptions _options;

		public BusHostedService(
			IBusControl busControl,
			SimplifiedBusHealthCheck simplifiedBusCheck,
			ReceiveEndpointHealthCheck receiveEndpointCheck,
			IOptions<BusOptions> options,
			ILogger<BusHostedService> logger)
		{
			_busControl = busControl;
			_simplifiedBusCheck = simplifiedBusCheck;
			_receiveEndpointCheck = receiveEndpointCheck;
			_options = options.Value;
			_logger = logger;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			_logger.LogInformation("Starting the bus...");

			_busControl.ConnectReceiveEndpointObserver(_receiveEndpointCheck);

			await _busControl.StartAsync(cancellationToken).ConfigureAwait(false);

			_simplifiedBusCheck.ReportBusStarted();

			_logger.LogInformation("Bus started");

		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Stopping bus...");

			if (_busControl != null)
			{
				await _busControl.StopAsync(cancellationToken);
			}

			_logger.LogInformation("Bus stopped");
		}
	}
}