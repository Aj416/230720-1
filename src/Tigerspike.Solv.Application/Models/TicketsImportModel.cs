using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tigerspike.Solv.Core.Services;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Request body when importing tickets.
	/// </summary>
	[ModelBinder(typeof(JsonWithFilesFormDataModelBinder), Name = "metadata")]
	public class TicketsImportModel
	{
		/// <summary>
		/// Metadata's with key value pair.
		/// </summary>
		public Dictionary<string, object> Metadata { get; set; }

		/// <summary>
		/// File to be uploaded with .csv extension.
		/// </summary>
		public IFormFile File { get; set; }
	}
}
