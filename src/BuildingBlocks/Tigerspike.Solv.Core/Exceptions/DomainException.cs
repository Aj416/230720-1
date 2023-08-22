using System;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tigerspike.Solv.Core.Exceptions
{
	/// <summary>
	/// Exception type for domain exceptions
	/// </summary>
	public class DomainException : Exception
	{
		public DomainException()
		{ }

		public DomainException(string message)
			: base(message)
		{ }

		public DomainException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}
