using System.Globalization;
using DotLiquid;

namespace Tigerspike.Solv.Core.Email
{
	public class TemplateOptions : RenderParameters
	{

		public TemplateOptions() : base(CultureInfo.CurrentCulture)
		{
			ErrorsOutputMode = ErrorsOutputMode.Display;
		}

	}
}