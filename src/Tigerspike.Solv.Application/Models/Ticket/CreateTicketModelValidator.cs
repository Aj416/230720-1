using FluentValidation;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Core.Extensions;
using Microsoft.FeatureManagement;

namespace Tigerspike.Solv.Application.Models.Ticket
{
	public class CreateTicketModelValidator : AbstractValidator<CreateTicketModel>
	{
		public CreateTicketModelValidator(IFeatureManager featureManager)
		{
			RuleFor(x => x.Question).NotEmpty();
			RuleFor(x => x.FirstName).MaximumLength(50);
			RuleFor(x => x.LastName).MaximumLength(50);
			RuleFor(x => x.Email).NotEmpty().EmailAddress();
			RuleFor(x => x.ReferenceId).MaximumLength(50);
			RuleFor(x => x.Source).MaximumLength(50);
			RuleFor(x => x.TransportType).Must(x => x.IsIn(TicketTransportType.Chat, TicketTransportType.Email));
		}

	}
}