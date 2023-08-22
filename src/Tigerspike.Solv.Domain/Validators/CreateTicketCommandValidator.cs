using System;
using FluentValidation;
using Tigerspike.Solv.Domain.Commands.Ticket;

namespace Tigerspike.Solv.Domain.Validators
{
	public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
	{
		public CreateTicketCommandValidator()
		{
			RuleFor(c => c.Email)
				.NotNull()
				.EmailAddress()
				.WithMessage("Email must be valid");

			RuleFor(c => c.FirstName)
				.MaximumLength(50)
				.WithMessage("Firstname must not be over 50 characters");

			RuleFor(c => c.LastName)
				.MaximumLength(50)
				.WithMessage("Lastname must not be over 50 characters");

			RuleFor(c => c.TransportType).IsInEnum();
			RuleFor(c => c.BrandId).NotEqual(Guid.Empty);
		}
	}
}
