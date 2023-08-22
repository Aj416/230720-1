namespace Tigerspike.Solv.Core.Exceptions
{
	public class ServiceException : System.Exception
	{
		public ServiceException(string message) : base(message)
		{ }

		public ServiceException(string message, System.Exception ex) : base(message, ex)
		{ }
	}
}