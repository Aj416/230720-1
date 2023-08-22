using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Identity;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;

namespace Tigerspike.Solv.Application.Services
{
	public class UserService : IUserService
	{
		private readonly IMapper _mapper;
		private readonly IMediatorHandler _mediatorHandler;
		private readonly IUserRepository _userRepository;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly ICachedAdvocateRepository _cachedAdvocateRepository;
		private readonly IAuthenticationService _authenticationService;

		public UserService(
			IMapper mapper,
			IMediatorHandler mediatorHandler,
			IAuthenticationService authenticationService,
			IUserRepository userRepository,
			IAdvocateRepository advocateRepository,
			ICachedAdvocateRepository cachedAdvocateRepository)
		{
			_mapper = mapper;
			_mediatorHandler = mediatorHandler;
			_authenticationService = authenticationService;
			_userRepository = userRepository;
			_advocateRepository = advocateRepository;
			_cachedAdvocateRepository = cachedAdvocateRepository;
		}

		/// <inheritdoc/>
		public async Task<UserModel> FindByUserId(Guid userId)
		{
			var userProfile = await _userRepository.FindAsync(userId);

			if (userProfile != null)
			{
				var userDto = _mapper.Map<UserModel>(userProfile);
				return userDto;
			}

			return null;
		}

		/// <inheritdoc/>
		public Task ChangePassword(Guid userId, string email, string oldPassword, string newPassword) =>
			_authenticationService.ChangePassword(userId, email, oldPassword, newPassword);

		/// <inheritdoc/>
		public async Task<Guid?> CreateAdmin(string firstName, string lastName, string email, string password) =>
			await _mediatorHandler.SendCommand(new CreateAdminIdentityCommand(Guid.NewGuid(), firstName, lastName, email, password));

		/// <inheritdoc/>
		public Task Block(Guid userId) => _mediatorHandler.SendCommand(new BlockUserCommand(userId));

		/// <inheritdoc/>
		public Task Unblock(Guid userId) => _mediatorHandler.SendCommand(new UnblockUserCommand(userId));

		/// <inheritdoc/>
		public async Task ChangeName(Guid userId, string firstName, string lastName) => await _mediatorHandler.SendCommand(new ChangeNameCommand(userId, firstName, lastName));

		/// <inheritdoc/>
		public async Task SetPhone(Guid userId, string phone) => await _mediatorHandler.SendCommand(new SetPhoneCommand(userId, phone));

		/// <inheritdoc/>
		public async Task SendVerificationMail(Guid userId) => await _mediatorHandler.SendCommand(new SendVerificationMailCommand(userId));

		/// <inheritdoc/>
		public async Task ResetMfa(Guid userId) => await _mediatorHandler.SendCommand(new ResetMfaCommand(userId));

		/// <inheritdoc/>
		public async Task<AccessLevel> GetAccessLevel(ClaimsPrincipal user)
		{
			if (user.IsInRole(SolvRoles.Admin))
				return AccessLevel.Admin;

			if (user.IsInRole(SolvRoles.Client))
				return AccessLevel.Client;

			if (user.IsInRole(SolvRoles.SuperSolver))
				return AccessLevel.SuperSolver;

			if (user.IsInRole(SolvRoles.Advocate))
			{
				var internalAgent = await _cachedAdvocateRepository.GetInternalAgentInfo(user.GetId());
				return internalAgent ? AccessLevel.InternalAgent : AccessLevel.RegularSolver;
			}

			return AccessLevel.None;
		}

	}
}