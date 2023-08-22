using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Tigerspike.Solv.Localization.Json.ResourcesModel;

namespace Tigerspike.Solv.Localization
{
	public interface ILocalizationProvider
	{
		CultureInfo CultureInfo { get; }
		ResourcesModel Resources { get; }
	}
}