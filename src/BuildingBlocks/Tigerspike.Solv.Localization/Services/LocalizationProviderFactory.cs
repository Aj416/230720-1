using System.Collections.Concurrent;
using System.Globalization;

namespace Tigerspike.Solv.Localization
{
	public class LocalizationProviderFactory : ILocalizationProviderFactory
	{
		private readonly ConcurrentDictionary<string, ILocalizationProvider> _providers = new ConcurrentDictionary<string, ILocalizationProvider>();
		public ILocalizationProvider GetLocalizationProvider(string culture) => GetLocalizationProvider(new CultureInfo(culture ?? "en-US"));
		public ILocalizationProvider GetLocalizationProvider(CultureInfo cultureInfo) => _providers.GetOrAdd(cultureInfo?.Name ?? "en-US", x => new LocalizationProvider(cultureInfo));
	}
}