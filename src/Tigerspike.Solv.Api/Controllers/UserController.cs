using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tigerspike.Solv.Api.Models;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// User Controller
	/// </summary>
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/users")]
	public class UserController : ApiController
	{
		private readonly IUserService _userService;
		private readonly IAdvocateService _advocateService;
		private readonly IBrandService _brandService;
		private readonly IClientService _clientService;

		/// <summary>
		/// User controller constructor
		/// </summary>
		public UserController(IUserService userService,
			IClientService clientService,
			IAdvocateService advocateService,
			IBrandService brandService,
			IDomainNotificationHandler notificationHandler,
			IMediatorHandler mediator
		) : base(notificationHandler, mediator)
		{
			_userService = userService;
			_advocateService = advocateService;
			_brandService = brandService;
			_clientService = clientService;
		}

		/// <summary>
		/// Get User
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(AdvocateModel), StatusCodes.Status200OK)]
		[Route("/v{version:apiVersion}/userInfo")]
		public async Task<IActionResult> GetUser()
		{
			if (User.IsInRole(SolvRoles.Advocate) || User.IsInRole(SolvRoles.SuperSolver))
			{
				var advocate = await _advocateService.FindAsync(User.GetId());

				if (advocate == null)
				{ notifyError(SolvRoles.Advocate); }

				return Response(advocate);
			}

			if (User.IsInRole(SolvRoles.Client))
			{
				var client = await _clientService.GetClient(User.GetId());

				if (client == null)
				{ notifyError(SolvRoles.Client); }

				return Response(client);
			}

			var user = await _userService.FindByUserId(User.GetId());
			if (user == null)
			{
				notifyError("User");

			}

			// In very rare cases, the user is authorized and has a role, but doesn't exist in BE.
			void notifyError(string role) => NotifyError(role, $"{role} doesn't exist", (int)HttpStatusCode.NotFound);

			return Response(user);
		}

		/// <summary>
		/// Change Password
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost("change-password")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
		{
			await _userService.ChangePassword(User.GetId(), User.GetEmail(), model.OldPassword, model.NewPassword);
			return Response(StatusCodes.Status204NoContent);
		}

		/// <summary>
		/// Returns list of brands associated with specified user
		/// </summary>
		/// <param name="userId">The user id</param>
		/// <returns></returns>
		[Authorize]
		[HttpGet("{userId}/brands")]
		[ProducesResponseType(typeof(IEnumerable<AdvocateBrandModel>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(IEnumerable<BrandModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAssociatedBrands(Guid userId)
		{
			if (User.IsInRole(SolvRoles.Admin))
			{
				return Response(await _brandService.GetForAdvocate(userId));
			}
			else
			{
				if (userId == User.GetId())
				{
					if (User.IsInRole(SolvRoles.Advocate) || User.IsInRole(SolvRoles.SuperSolver))
					{
						return Response(await _brandService.GetForAdvocate(userId));
					}

					if (User.IsInRole(SolvRoles.Client))
					{
						return Response(new[] { await _brandService.GetClientBrand(userId) });
					}

					return BadRequest();
				}
				else
				{
					return Forbid();
				}
			}
		}

		/// <summary>
		/// Blocks a user.
		/// </summary>
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpPost("{userId}/block")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Block(Guid userId)
		{
			if (userId != User.GetId())
			{
				await _userService.Block(userId);
				return Response();
			}
			else
			{
				return BadRequest("You cannot block yourself");
			}
		}

		/// <summary>
		/// Blocks a user.
		/// </summary>
		[Authorize(Roles = SolvRoles.Admin)]
		[HttpPost("{userId}/unblock")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Unblock(Guid userId)
		{
			await _userService.Unblock(userId);
			return Response();
		}

		/// <summary>
		/// Change Name
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost("change-name")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> ChangeName([FromBody] ChangeNameModel model)
		{
			if (User.IsInRole(SolvRoles.Advocate))
			{
				await _userService.ChangeName(User.GetId(), model.FirstName, model.LastName);
				return Response(StatusCodes.Status204NoContent);
			}
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Change user's phone
		/// </summary>
		/// <param name="userId">The user's id</param>
		/// <param name="model">The data change request model</param>
		/// <returns></returns>
		[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver)]
		[HttpPost("{userId}/phone")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> SetPhone(Guid userId, [FromBody] SetPhoneModel model)
		{
			if (User.GetId() == userId)
			{
				await _userService.SetPhone(userId, model.Phone);
				return Response();
			}
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Send verification email
		/// </summary>
		/// <param name="userId">The user's id</param>
		/// <returns></returns>
		[HttpPost("{userId}/sendverificationmail")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> SendVerificationEmail(Guid userId)
		{
			await _userService.SendVerificationMail(userId);
			return Response();
		}

		/// <summary>
		/// Send verification email
		/// </summary>
		/// <param name="userId">The user's id</param>
		/// <returns></returns>
		[HttpDelete("{userId}/mfa")]
		[Authorize(Roles = SolvRoles.Admin)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> ResetMfa(Guid userId)
		{
			await _userService.ResetMfa(userId);
			return Response();
		}
	}
}