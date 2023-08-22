using System.Collections.Generic;

namespace Tigerspike.Solv.Core.Email
{
	public interface ITemplateService
	{
		/// <summary>
		/// Renders a template with a supplied data
		/// </summary>
		/// <param name="template">Template</param>
		/// <param name="inputs">Input data</param>
		string Render(string template, params object[] inputs);
	}
}