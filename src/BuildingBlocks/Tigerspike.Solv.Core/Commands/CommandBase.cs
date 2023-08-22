using System;
using FluentValidation.Results;
using MediatR;

namespace Tigerspike.Solv.Core.Commands
{
	public abstract class CommandBase
	{
		public DateTime Timestamp { get; private set; }

		public string MessageType { get; private set; }

		public ValidationResult ValidationResult { get; protected set; }

		public abstract bool IsValid();

		protected CommandBase(string messageType = null)
		{
			Timestamp = DateTime.Now;
			MessageType = messageType ?? GetType().Name;
		}
	}
}
