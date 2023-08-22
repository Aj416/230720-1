using System.Collections.Generic;
using MassTransit;

namespace Tigerspike.Solv.Infra.Bus.Configuration
{
	public interface IBusTransportConfiguration
	{
		IBusControl Configure(List<BusEndpointConfiguration> endpoints);
	}
}