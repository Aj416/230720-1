using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEmail.Core.Models;

namespace Tigerspike.Solv.Services.Notification.Application.Services
{
	public interface IEmailService
	{
		/// <summary>
		/// Sends an email using a template and a model.
		/// </summary>
		/// <param name="replyTo">The reply to email address.</param>
		/// <param name="mailTo">The destination email address.</param>
		/// <param name="subject">The email subject.</param>
		/// <param name="template">The name of the email template.</param>
		/// <param name="model">The template model.</param>
		/// <param name="culture">The culture of the email.</param>
		/// <param name="attachment">Optional email attachment.</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		Task<bool> SendAsync(string replyTo, string mailTo, string subject, string template, Dictionary<string, object> model, string culture, string senderName = null, Attachment attachment = null);
	}
}