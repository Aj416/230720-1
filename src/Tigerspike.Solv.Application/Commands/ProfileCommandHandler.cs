using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.CommandHandlers
{
	public class ProfileCommandHandler : CommandHandler,
		IRequestHandler<SubmitProfileAnswersCommand, bool>
	{
		private readonly IAdvocateApplicationRepository _advocateApplicationRepository;
		private readonly IApplicationAnswerRepository _applicationAnswerRepository;

		public ProfileCommandHandler(
			IAdvocateApplicationRepository advocateApplicationRepository,
			IApplicationAnswerRepository applicationAnswerRepository,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			IDomainNotificationHandler notifications) : base(uow, mediator, notifications)
		{
			_advocateApplicationRepository = advocateApplicationRepository ??
											 throw new ArgumentNullException(nameof(advocateApplicationRepository));
			_applicationAnswerRepository = applicationAnswerRepository;
		}

		public async Task<bool> Handle(SubmitProfileAnswersCommand request, CancellationToken cancellationToken)
		{
			var advocateApplication = await _advocateApplicationRepository.FindAsync(request.AdvocateApplicationId);

			if (advocateApplication == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"The advocate application does not exists"));
				return false;
			}

			var hasAnswers = await _applicationAnswerRepository.HasAnswers(request.AdvocateApplicationId);

			if (hasAnswers)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"The advocate application has already been completed"));
				return false;
			}

			advocateApplication.SetAnswers(request.ApplicationAnswers);

			await _applicationAnswerRepository.InsertAsync(request.ApplicationAnswers, cancellationToken);

			if (await Commit())
			{
				// Raise event
			}

			return true;
		}
	}
}