using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
#pragma warning disable 109

namespace Tigerspike.Solv.Core.Mvc
{
	/// <summary>
	/// API Controller
	/// </summary>
	[ApiController]
	public abstract class ApiController : ControllerBase
	{
		private readonly IDomainNotificationHandler _notifications;
		protected readonly IMediatorHandler _mediator;

		/// <summary>
		/// API Constructor
		/// </summary>
		/// <param name="notifications"></param>
		/// <param name="mediator"></param>
		protected ApiController(IDomainNotificationHandler notifications,
			IMediatorHandler mediator)
		{

			_notifications = notifications;
			_mediator = mediator;

		}

		/// <summary>
		/// Get's Notifications
		/// </summary>
		protected IEnumerable<DomainNotification> Notifications => _notifications.GetNotifications();

		protected async Task<string> GetRequestPayload()
		{
			using (var reader = new StreamReader(Request.Body))
			{
				return await reader.ReadToEndAsync();
			}
		}

		/// <summary>
		/// ///
		/// </summary>
		/// <param name="statusCode"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		protected new IActionResult Response(int statusCode, object result = null)
		{
			if (IsValidOperation())
			{
				return StatusCode(statusCode, result);
			}
			else
			{
				return Response(result);
			}
		}

		/// <summary>
		/// ///
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		protected new IActionResult Response(object result = null)
		{
			if (IsValidOperation())
			{
				return result != null ? Ok(result) : (IActionResult)NoContent();
			}
			else
			{
				var customStatusCodeResponse = _notifications.GetNotifications()
					.Where(x => x.Code != 0)
					.FirstOrDefault();

				var allNotifications = _notifications.GetNotifications().Select(n => n.Value).Aggregate((a, b) => a + ", " + b);

				var problemDetails = new ProblemDetails()
				{
					Status = customStatusCodeResponse?.Code ?? (int)HttpStatusCode.BadRequest,
					// TODO: This has to reflect more meaningful title (like Ticket Unavailable)
					Title = "Invalid request",
					Detail = customStatusCodeResponse?.Value ?? allNotifications,
				};
				return StatusCode(problemDetails.Status.Value, problemDetails);
			}
		}

		/// <summary>
		/// ///
		/// </summary>
		protected void NotifyModelStateErrors()
		{
			var errors = ModelState.Values.SelectMany(v => v.Errors);
			foreach (var error in errors)
			{
				var errorMsg = error.Exception == null ? error.ErrorMessage : error.Exception.Message;
				NotifyError(string.Empty, errorMsg);
			}
		}

		/// <summary>
		/// ///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="message"></param>
		/// <param name="code"></param>
		protected void NotifyError(string key, string message, int code = 0) => _mediator.RaiseEvent(new DomainNotification(key, message, code));

		/// <summary>
		/// Checks if Valid Operation
		/// </summary>
		/// <returns></returns>
		private bool IsValidOperation() => !_notifications.HasNotifications();
	}
}