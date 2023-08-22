using AutoMoqCore;
using Tigerspike.Solv.Domain.Commands.Ticket;

namespace Tigerspike.Solv.Application.Tests.Validators
{
	public abstract class ImportTicketCommandValidatorBase
	{
		protected readonly AutoMoqer Mocker = new AutoMoqer();
		protected ImportTicketCommandValidator SystemUnderTest => Mocker.Create<ImportTicketCommandValidator>();
	}
}