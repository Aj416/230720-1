using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.Core.Exceptions;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Exceptions;

namespace Tigerspike.Solv.Auth0
{
	public class AuthenticationService : IAuthenticationService
	{
		private const string Auth0Connection = "Username-Password-Authentication";
		private const string InvalidPasswordError = "invalid_grant";
		private const string MfaRequiredError = "mfa_required";
		private readonly Auth0Options _auth0Options;
		private readonly AuthenticationApiClient _authClient;
		private readonly ILogger<AuthenticationService> _logger;

		public AuthenticationService(IOptions<Auth0Options> auth0Options, ILogger<AuthenticationService> logger)
		{
			_auth0Options = auth0Options.Value;
			_authClient = new AuthenticationApiClient(new Uri(_auth0Options.ManagementApi.Authority).Host);
			_logger = logger;
		}

		public async Task CreateAdvocate(Guid id, string firstName, string lastName, string email, string password) =>
			await CreateAccount(id, firstName, lastName, email, password, SolvRoles.Advocate);

		public async Task CreateClient(Guid id, string firstName, string lastName, string email, string password) =>
			await CreateAccount(id, firstName, lastName, email, password, SolvRoles.Client);

		public async Task
			CreateSuperSolver(Guid id, string firstName, string lastName, string email, string password) =>
			await CreateAccount(id, firstName, lastName, email, password, SolvRoles.SuperSolver);

		public async Task CreateAdmin(Guid id, string firstName, string lastName, string email, string password) =>
			await CreateAccount(id, firstName, lastName, email, password, SolvRoles.Admin);


		public async Task<bool> IsPasswordValid(string email, string password)
		{
			try
			{
				await _authClient.GetTokenAsync(new ResourceOwnerTokenRequest
				{
					Audience = _auth0Options.ManagementApi.Audience,
					ClientId = _auth0Options.ManagementApi.ClientId,
					ClientSecret = _auth0Options.ManagementApi.ClientSecret,
					Username = email,
					Scope = "openid profile",
					Password = password
				});

				return true;
			}
			catch(ErrorApiException ex)
			{
				return (ex.ApiError.Error) switch
				{
					InvalidPasswordError => false,
					MfaRequiredError => true, // if MFA is enabled on the tenant, that's ok, it means the provided password was still valid
					_ => throw ex, // unhandled case - propagate the exception
				};
			}
		}

		public async Task ChangePassword(Guid userId, string email, string oldPassword, string newPassword)
		{
			if (await IsPasswordValid(email, oldPassword))
			{
				using var apiClient = GetManagementApiClient();
				await apiClient.Users
					.UpdateAsync(GetAuth0UserId(userId), new UserUpdateRequest { Password = newPassword });
			}
			else
			{
				throw new ServiceInvalidOperationException("Wrong password or email");
			}
		}

		private async Task CreateAccount(Guid id, string firstName, string lastName, string email, string password,
			string role)
		{
			try
			{
				await CreateUser(id, email, firstName, lastName, password);
				await AssignRoleToUser(id, role);
			}
			catch (ApiException ex)
			{
				// Throw a friendly exception so the caller can intercept it.
				throw new ServiceInvalidOperationException(ex.Message, ex);
			}
		}

		private async Task CreateUser(Guid id, string email, string firstName, string lastName, string password)
		{
			using var apiClient = GetManagementApiClient();
			await apiClient.Users.CreateAsync(new UserCreateRequest
			{
				UserId = id.ToString(),
				FirstName = firstName,
				LastName = lastName,
				Email = email,
				VerifyEmail = true,
				Connection = Auth0Connection,
				Password = password,
			});
		}

		private async Task AssignRoleToUser(Guid userId, string solvRole)
		{
			using var apiClient = GetManagementApiClient();
			var roles = await apiClient.Roles.GetAllAsync(new GetRolesRequest { NameFilter = solvRole });
			var role = roles.Single();
			await apiClient.Roles.AssignUsersAsync(role.Id,
				new AssignUsersRequest { Users = new[] { GetAuth0UserId(userId) } });
		}

		public async Task<IEnumerable<string>> GetUsersInRole(string roleName)
		{
			using var apiClient = GetManagementApiClient();
			var roles = await apiClient.Roles.GetAllAsync(new GetRolesRequest { NameFilter = roleName });
			var role = roles.Single();
			return (await apiClient.Roles.GetUsersAsync(role.Id)).Select(u => u.UserId);
		}

		public async Task Block(Guid userId)
		{
			using var apiClient = GetManagementApiClient();
			try
			{
				await apiClient.Users.UpdateAsync(GetAuth0UserId(userId), new UserUpdateRequest { Blocked = true });
			}
			catch (ApiException ex)
			{
				// Throw a friendly exception so the caller can intercept it.
				throw new ServiceInvalidOperationException(ex.Message, ex);
			}
		}

		private ManagementApiClient GetManagementApiClient()
		{
			var response = _authClient.GetTokenAsync(new ClientCredentialsTokenRequest
			{
				ClientId = _auth0Options.ManagementApi.ClientId,
				ClientSecret = _auth0Options.ManagementApi.ClientSecret,
				Audience = _auth0Options.ManagementApi.Audience
			});

			var token = response.Result.AccessToken;
			return new ManagementApiClient(token, new Uri(_auth0Options.ManagementApi.Audience));
		}

		public async Task ChangeName(Guid userId, string firstname, string lastName)
		{
			using var apiClient = GetManagementApiClient();
			try
			{
				await apiClient.Users.UpdateAsync(GetAuth0UserId(userId), new UserUpdateRequest { FirstName = firstname, LastName = lastName });
			}
			catch (ApiException ex)
			{
				// Throw a friendly exception so the caller can intercept it.
				throw new ServiceInvalidOperationException(ex.Message, ex);
			}
		}

		public async Task SendVerificationEmail(Guid userId, string emailId)
		{
			using var apiClient = GetManagementApiClient();
			try
			{
				await apiClient.Users.UpdateAsync(GetAuth0UserId(userId), new UserUpdateRequest { VerifyEmail = true, Email = emailId });
			}
			catch (ApiException ex)
			{
				// Throw a friendly exception so the caller can intercept it.
				throw new ServiceInvalidOperationException(ex.Message, ex);
			}
		}

		/// <inheritdoc/>
		public async Task<bool> ResetMfa(Guid userId)
		{
			using var apiClient = GetManagementApiClient();

			try
			{
				var enrollments = await apiClient.Users.GetEnrollmentsAsync(GetAuth0UserId(userId));
				if (enrollments.Count > 0)
				{
					foreach (var enrollment in enrollments)
					{
						await apiClient.Guardian.DeleteEnrollmentAsync(enrollment.Id);
					}

					return true;
				}
				else
				{
					return false;
				}
			}
			catch (ApiException ex)
			{
				// Throw a friendly exception so the caller can intercept it.
				throw new ServiceInvalidOperationException(ex.Message, ex);
			}
		}

		public async Task MarkAllEmailsAsVerified()
		{
			using var apiClient = GetManagementApiClient();
			IPagedList<User> users = null;

			do
			{
				_logger.LogDebug("Retrieving users without email verified");
				await HandlingPolicy.ExecuteAsync(async () =>
				{
					// we can always get first page, because each record we update would no longer be returned by that query
					users = await apiClient.Users.GetAllAsync(new GetUsersRequest()
					{
						Fields = "user_id,email,email_verified",
						Query = "email_verified:false",
						IncludeFields = true,
					}, new PaginationInfo());
				});
				_logger.LogDebug($"Retrieved {users.Count} users without email verified");

				foreach (var user in users)
				{
					await HandlingPolicy.ExecuteAsync(async () =>
					{
						_logger.LogDebug($"Updating user {user.Email} to be marked as email verified");
						await apiClient.Users.UpdateAsync(user.UserId, new UserUpdateRequest() { EmailVerified = true });
						_logger.LogDebug($"User {user.Email} is now marked as email verified");
					});
				}
			} while (users.Count > 0);

			_logger.LogDebug("Marking all users email verified is completed");
		}

		private string GetAuth0UserId(Guid userId) => $"auth0|{userId}";
		private IAsyncPolicy HandlingPolicy => Polly.Policy
			.Handle<RateLimitApiException>()
			.WaitAndRetryAsync(3, (attempt, ex, ctx) =>
			{
				var rateEx = ex as RateLimitApiException;
				return (rateEx?.RateLimit.Reset - DateTimeOffset.Now) ?? TimeSpan.Zero;
			}, (ex, delay, attempt, ctx) => Task.CompletedTask);
	}
}