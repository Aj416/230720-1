using System;
using FluentValidation;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Tickets export request validator
	/// </summary>
	public class TicketExportRequestModelValidator : AbstractValidator<TicketExportRequestModel>
	{
		/// <summary>
		/// Validator logic
		/// </summary>
		public TicketExportRequestModelValidator()
		{
			RuleFor(c => c.From)
				.Must((model, from) => ((model.To ?? DateTime.UtcNow.Date) - from)?.TotalDays <= 90)
				.WithMessage("Selected period has to be no longer than 90 days");
		}
	}
}