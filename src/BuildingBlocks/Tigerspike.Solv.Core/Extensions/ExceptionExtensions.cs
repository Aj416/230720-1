using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Core.Extensions
{
	public static class ExceptionExtensions
	{
		public static IEnumerable<Exception> WithInnerExceptions(this Exception exception)
		{
			var ex = exception;
			while (ex != null)
			{
				yield return ex;
				ex = ex.InnerException;
			}
		}
	}
}