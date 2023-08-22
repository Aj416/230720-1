using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface IAuthenticationService
	{
		/// <summary>
		/// Create an advocate in the underlying authentication service
		/// </summary>
		/// <param name="id">The id of the advocate to be created</param>
		/// <param name="firstName">The first name</param>
		/// <param name="lastName">The last name</param>
		/// <param name="email">The email</param>
		/// <param name="password">The password</param>
		/// <returns></returns>
		Task CreateAdvocate(Guid id, string firstName, string lastName, string email, string password);

		/// <summary>
		/// Create an client in the underlying authentication service
		/// </summary>
		/// <param name="id">The id of the client to be created</param>
		/// <param name="firstName">The first name</param>
		/// <param name="lastName">The last name</param>
		/// <param name="email">The email</param>
		/// <param name="password">The password</param>
		/// <returns></returns>
		Task CreateClient(Guid id, string firstName, string lastName, string email, string password);

		/// <summary>
		/// Create a SuperSolver in the underlying authentication service
		/// </summary>
		/// <param name="id">The id of the account to be created</param>
		/// <param name="firstName">The first name</param>
		/// <param name="lastName">The last name</param>
		/// <param name="email">The email</param>
		/// <param name="password">The password</param>
		/// <returns></returns>
		Task CreateSuperSolver(Guid id, string firstName, string lastName, string email, string password);

		/// <summary>
		/// Create a Admin in the underlying authentication service
		/// </summary>
		/// <param name="id">The id of the account to be created</param>
		/// <param name="firstName">The first name</param>
		/// <param name="lastName">The last name</param>
		/// <param name="email">The email</param>
		/// <param name="password">The password</param>
		/// <returns></returns>
		Task CreateAdmin(Guid id, string firstName, string lastName, string email, string password);

		/// <summary>
		/// Change the password of the user with the specificed id ///
		/// </summary>
		/// <param name="userId">The user id</param>
		/// <param name="email">The email of the user</param>
		/// <param name="oldPassword">The old password to be checked</param>
		/// <param name="newPassword">The new password to be updated</param>
		/// <returns></returns>
		Task ChangePassword(Guid userId, string email, string oldPassword, string newPassword);

		/// <summary>
		/// Block the user specified.
		/// </summary>
		Task Block(Guid userId);

		/// <summary>
		/// Gets the list of ids of users assigned to the role.
		/// </summary>
		/// <param name="roleName">The role name.</param>
		/// <returns>The list of ids of users.</returns>
		Task<IEnumerable<string>> GetUsersInRole(string roleName);

		/// <summary>
		/// Change the name of the user.
		/// </summary>
		/// <param name="userId">The user id</param>
		/// <param name="firstname">The first name of the user.</param>
		/// <param name="lastName">The last name of the user.</param>
		/// <returns></returns>
		Task ChangeName(Guid userId, string firstname, string lastName);

		/// <summary>
		/// Send Verification email
		/// </summary>
		/// <param name="userId">The user id</param>
		/// <returns></returns>
		Task SendVerificationEmail(Guid userId, string emailId);

		/// <summary>
		/// Reset user's MFA enrollment (if possible)
		/// </summary>
		/// <param name="userId">The user id</param>
		/// <returns>true if enrollment was deleted succesfully, false otherwise</returns>
		Task<bool> ResetMfa(Guid userId);
		Task MarkAllEmailsAsVerified();
	}
}