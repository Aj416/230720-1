using System.Globalization;
using System;
using Tigerspike.Solv.Localization.Json.ResourcesModel;
using Newtonsoft.Json.Linq;

namespace Tigerspike.Solv.Localization
{
	public class LocalizationProvider : ILocalizationProvider
	{
		public LocalizationProvider(CultureInfo cultureInfo)
		{
			CultureInfo = cultureInfo;

			var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}/Resources";
			var defaultModel = GetModel($"{basePath}/ResourcesModel.json");
			var specificModel = GetModel($"{basePath}/{cultureInfo?.Name}.json");

			defaultModel.Merge(specificModel);
			Resources = defaultModel.ToObject<ResourcesModel>();
		}

		public CultureInfo CultureInfo { get; }
		public ResourcesModel Resources { get; }

		private JObject GetModel(string filePath)
		{
			if (System.IO.File.Exists(filePath))
			{
				var raw = System.IO.File.ReadAllText(filePath);
				return JObject.Parse(raw);
			}
			else
			{
				return new JObject();
			}
		}
	}
}