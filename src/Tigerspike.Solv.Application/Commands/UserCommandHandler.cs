using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Exceptions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Identity;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Events.User;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.CommandHandlers
{
	public class UserCommandHandler : CommandHandler,
		IRequestHandler<CreateClientIdentityCommand, Guid?>,
		IRequestHandler<CreateClientCommand, Unit>,
		IRequestHandler<CreateSuperSolverIdentityCommand, Guid?>,
		IRequestHandler<CreateAdminIdentityCommand, Guid?>,
		IRequestHandler<CreateAdminCommand, Unit>,
		IRequestHandler<ChangeAdvocateNameCommand, Unit>,
		IRequestHandler<ResetMfaCommand, Unit>,
		IRequestHandler<BlockUserCommand, Unit>,
		IRequestHandler<ChangeNameCommand, Unit>,
		IRequestHandler<UnblockUserCommand, Unit>,
		IRequestHandler<SetPhoneCommand, Unit>,
		IRequestHandler<SendVerificationMailCommand, Unit>
	{
		private readonly IUserRepository _userRepository;
		private readonly IClientRepository _clientRepository;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IAdvocateApplicationRepository _advocateApplicationRepository;
		private readonly IAuthenticationService _authenticationService;

		public UserCommandHandler(
			IUserRepository userRepository,
			IClientRepository clientRepository,
			IAdvocateRepository advocateRepository,
			IAdvocateApplicationRepository advocateApplicationRepository,
			IAuthenticationService authenticationService,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			IDomainNotificationHandler notifications) : base(uow, mediator, notifications)
		{
			_userRepository = userRepository;
			_clientRepository = clientRepository;
			_advocateRepository = advocateRepository;
			_advocateApplicationRepository = advocateApplicationRepository;
			_authenticationService = authenticationService;
		}

		public async Task<Guid?> Handle(CreateClientIdentityCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == request.UserId || x.Email == request.Email);
			var client = await _clientRepository.FindAsync(request.UserId);

			if (user == null && client == null)
			{
				try
				{
					await _authenticationService.CreateClient(request.UserId, request.FirstName, request.LastName, request.Email, request.Password);
					await _mediator.RaiseEvent(new ClientIdentityCreatedEvent(request.UserId, request.BrandId, request.FirstName, request.LastName, request.Email, request.Phone));
					return request.UserId;
				}
				catch (ServiceInvalidOperationException ex)
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"[{request.Email}] {ex.Message}"));
					return null;
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"The client/user {request.UserId} ({request.Email}) already exists"));
				return null;
			}
		}

		public async Task<Unit> Handle(CreateClientCommand request, CancellationToken cancellationToken)
		{
			var user = new User(request.UserId, request.FirstName, request.LastName, request.Email, request.Phone);
			var client = new Client(user, request.BrandId);
			await _clientRepository.InsertAsync(client, cancellationToken);

			if (await Commit())
			{
				// TODO raise events if we want
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to save client/user {request.UserId} ({request.Email})"));
			}

			return Unit.Value;
		}

		public async Task<Guid?> Handle(CreateSuperSolverIdentityCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == request.UserId || x.Email == request.Email);
			var solver = await _advocateRepository.FindAsync(request.UserId);

			if (user == null && solver == null)
			{
				try
				{
					await _authenticationService.CreateSuperSolver(request.UserId, request.FirstName, request.LastName, request.Email, request.Password);
					await _mediator.RaiseEvent(new SuperSolverIdentityCreatedEvent(request.UserId, request.FirstName, request.LastName, request.Email, request.CountryCode, request.Phone));
					return request.UserId;
				}
				catch (ServiceInvalidOperationException ex)
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"[{request.Email}] {ex.Message}"));
					return null;
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"The solver/user {request.UserId} ({request.Email}) already exists"));
				return null;
			}
		}

		public async Task<Guid?> Handle(CreateAdminIdentityCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == request.UserId || x.Email == request.Email);

			if (user == null)
			{
				try
				{
					await _authenticationService.CreateAdmin(request.UserId, request.FirstName, request.LastName, request.Email, request.Password);
					await _mediator.RaiseEvent(new AdminIdentityCreatedEvent(request.UserId, request.FirstName, request.LastName, request.Email));
					return request.UserId;
				}
				catch (ServiceInvalidOperationException ex)
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"[{request.Email}] {ex.Message}"));
					return null;
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"The admin {request.UserId} ({request.Email}) already exists"));
				return null;
			}
		}

		public async Task<Unit> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.FindAsync(request.UserId);

			if (user != null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The admin / user already exists"));
				return Unit.Value;
			}

			user = new User(request.UserId, request.FirstName, request.LastName, request.Email, phone: null);

			await _userRepository.InsertAsync(user, cancellationToken);

			if (await Commit())
			{
				// TODO raise events if we want
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to save admin / user {request.UserId} ({request.Email})"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(BlockUserCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.FindAsync(request.UserId);

			if (user == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The user doesn't exists"));
				return Unit.Value;
			}

			user.Block();
			_userRepository.Update(user);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new UserBlockedEvent(user.Id));

			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to to block user {request.UserId}"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.FindAsync(request.UserId);

			if (user == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The user doesn't exists"));
				return Unit.Value;
			}

			user.Unblock();
			_userRepository.Update(user);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new UserUnblockedEvent(user.Id));

			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to unblock user {request.UserId}"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(ChangeNameCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.FindAsync(request.UserId);

			if (user != null)
			{
				try
				{
					await _authenticationService.ChangeName(request.UserId, request.FirstName, request.LastName);
					await _mediator.RaiseEvent(new NameChangedEvent(request.UserId, request.FirstName, request.LastName));
					return Unit.Value;
				}
				catch (ServiceInvalidOperationException ex)
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"[{request.UserId}] {ex.Message}"));
					return Unit.Value;
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The user doesn't exists"));
				return Unit.Value;
			}
		}

		public async Task<Unit> Handle(SendVerificationMailCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.FindAsync(request.UserId);

			if (user != null)
			{
				try
				{
					await _authenticationService.SendVerificationEmail(request.UserId, user.Email);
					return Unit.Value;
				}
				catch (ServiceInvalidOperationException ex)
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"[{request.UserId}] {ex.Message}"));
					return Unit.Value;
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The user doesn't exists"));
				return Unit.Value;
			}
		}

		public async Task<Unit> Handle(ChangeAdvocateNameCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.FindAsync(request.AdvocateId);

			if (user == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The user doesn't exists"));
				return Unit.Value;
			}

			user.ChangeName(request.FirstName, request.LastName);
			_userRepository.Update(user);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocateNameChangedEvent(request.AdvocateId));
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to change name of user {request.AdvocateId}"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetPhoneCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.FindAsync(request.UserId);
			var advocateApplication = await _advocateApplicationRepository.FindAsync(request.UserId);

			if (user == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The user doesn't exists"));
				return Unit.Value;
			}

			user.SetPhone(request.Phone);
			_userRepository.Update(user);

			if (advocateApplication != null)
			{
				advocateApplication.SetPhone(request.Phone);
				_advocateApplicationRepository.Update(advocateApplication);
			}

			if (await Commit())
			{
				await _mediator.RaiseEvent(new PhoneChangedEvent(request.UserId, request.Phone));
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to set phone of user {request.UserId}"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(ResetMfaCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.FindAsync(request.UserId);
			if (user != null)
			{
				await _authenticationService.ResetMfa(request.UserId);
				await _mediator.RaiseEvent(new MfaResetEvent(request.UserId, user.Email));
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"User not found: {request.UserId}"));
			}

			return Unit.Value;
		}
	}
}