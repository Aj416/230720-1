using System.Globalization;

namespace Tigerspike.Solv.Localization
{
	public interface ILocalizationProviderFactory
	{
		ILocalizationProvider GetLocalizationProvider(string culture);
		ILocalizationProvider GetLocalizationProvider(CultureInfo cultureInfo);
	}
}