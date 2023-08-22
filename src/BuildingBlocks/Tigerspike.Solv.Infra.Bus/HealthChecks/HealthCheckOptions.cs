using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Tigerspike.Solv.Infra.Bus.HealthChecks
{
    public class HealthCheckOptions
    {
        public string BusHealthCheckName { get; set; }
        public string ReceiveEndpointHealthCheckName { get; set; }
        public string PingHealthCheckName { get; set; }
        public HealthStatus FailureStatus { get; set; }
        public IEnumerable<string> Tags { get; set; }

        public static HealthCheckOptions Default
            => new HealthCheckOptions
            {
                BusHealthCheckName = "bus",
                ReceiveEndpointHealthCheckName = "bus-endpoint",
                PingHealthCheckName = "bus-ping",
                FailureStatus = HealthStatus.Unhealthy,
                Tags = new[] { "ready" }
            };
    }
}