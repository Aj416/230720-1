using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface IUserService
	{
		/// <summary>
		/// Returns the user information based on the id.
		/// </summary>
		/// <param name="userId">The identifier of the user</param>
		/// <returns>User DTO that holds user information</returns>
		Task<UserModel> FindByUserId(Guid userId);

		/// <summary>
		/// Change the password of the logged in user.
		/// </summary>
		/// <param name="guid">The user id of the password to be changed</param>
		/// <param name="email">Email of user</param>
		/// <param name="oldPassword">Old password</param>
		/// <param name="newPassword">New password</param>
		Task ChangePassword(Guid userId, string email, string oldPassword, string newPassword);

		/// <summary>
		/// Create admin.
		/// </summary>
		/// <param name="firstName">The first name of admin</param>
		/// <param name="lastName">The last name of admin</param>
		/// <param name="email">The email of admin</param>
		/// <param name="password">The password of admin</param>
		/// <returns>Newly created admin Id</returns>
		Task<Guid?> CreateAdmin(string firstName, string lastName, string email, string password);

		/// <summary>
		/// Blocks a user be it Admin, Client, or Advocate.
		/// </summary>
		/// <param name="userId"></param>
		Task Block(Guid userId);

		/// <summary>
		/// Unblocks a user be it Admin, Client, or Advocate.
		/// </summary>
		/// <param name="userId"></param>
		Task Unblock(Guid userId);

		/// <summary>
		/// Change the password of the logged in user.
		/// </summary>
		/// <param name="userId">The user id of advocate.</param>
		/// <param name="firstName">The first name of the user.r</param>
		/// <param name="lastName">The last name of the user.</param>
		/// <returns></returns>
		Task ChangeName(Guid userId, string firstName, string lastName);

		/// <summary>
		/// Change the password of the logged in user.
		/// </summary>
		/// <param name="userId">The user id of advocate.</param>
		/// <returns></returns>
		Task SendVerificationMail(Guid userId);

		/// <summary>
		/// Get the cached info about user's access level
		/// </summary>
		/// <param name="user">The user</param>
		Task<AccessLevel> GetAccessLevel(ClaimsPrincipal user);

		/// <summary>
		/// Set's new phone for the user
		/// </summary>
		/// <param name="userId">The user id</param>
		/// <param name="phone">The new phone number</param>
		Task SetPhone(Guid userId, string phone);

		/// <summary>
		/// Reset MFA of the user
		/// </summary>
		/// <param name="userId">The user id of advocate.</param>
		Task ResetMfa(Guid userId);
	}
}