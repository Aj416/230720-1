using Newtonsoft.Json;

namespace Tigerspike.Solv.Core.Services
{
	public class ConfiguredJsonSerializer : IConfiguredJsonSerializer
	{
		private readonly JsonSerializerSettings _settings;
		public ConfiguredJsonSerializer(JsonSerializerSettings settings)
		{
			_settings = settings;
		}

		public T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _settings);

		public string Serialize(object data) => JsonConvert.SerializeObject(data, _settings);
	}
}