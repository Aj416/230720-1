using System;
using Tigerspike.Solv.Core.Bus;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SendProfilingReminderCommand : IScheduledJob
	{
		public string JobId => $"{nameof(SendProfilingReminderCommand)}-{AdvocateApplicationId}";
		public Guid AdvocateApplicationId { get; set; }
		public string Email { get; set; }

		/// <summary>
		/// Empty constructor, so MassTransit can deserialize the message
		/// </summary>
		protected SendProfilingReminderCommand()
		{

		}

		public SendProfilingReminderCommand(Guid advocateApplicationId, string email)
		{
			AdvocateApplicationId = advocateApplicationId;
			Email = email;
		}
	}
}