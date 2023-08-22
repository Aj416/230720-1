namespace Tigerspike.Solv.Core.Exceptions
{
	public class ServiceInvalidOperationException : ServiceException
	{
		public ServiceInvalidOperationException(string message) : base(message)
		{

		}

		public ServiceInvalidOperationException(string message, System.Exception ex) : base(message, ex)
		{

		}
	}
}