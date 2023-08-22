using System;
using System.Collections.Generic;
using MassTransit;

namespace Tigerspike.Solv.Infra.Bus.Configuration
{
	public class BusEndpointConfiguration
	{
		public string QueueName { get; set; }
		public Action<IReceiveEndpointConfigurator> EndpointConfigurator { get; set; }
	}
}