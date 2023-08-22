using System;

namespace Tigerspike.Solv.Infra.Bus.Exceptions
{
	/// <summary>
	/// Exception type for domain exceptions
	/// </summary>
	public class BusConfigurationException : Exception
	{
		public BusConfigurationException()
		{ }

		public BusConfigurationException(string message)
			: base(message)
		{ }

		public BusConfigurationException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}
