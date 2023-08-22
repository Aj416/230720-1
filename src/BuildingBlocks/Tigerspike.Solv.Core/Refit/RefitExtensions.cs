using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;

namespace Tigerspike.Solv.Core.Refit
{
	public static class RefitExtensions
	{
		public static T For<T>(string hostUrl) => RestService.For<T>(hostUrl, GetNewtonsoftJsonRefitSettings());
		public static T For<T>(HttpClient client) => RestService.For<T>(client, GetNewtonsoftJsonRefitSettings());
		public static T For<T>(string hostUrl, JsonSerializerSettings settings) => RestService.For<T>(hostUrl, new RefitSettings()
		{
			ContentSerializer = new NewtonsoftJsonContentSerializer(settings)
		});

		public static RefitSettings GetNewtonsoftJsonRefitSettings() =>
			new RefitSettings(new NewtonsoftJsonContentSerializer(new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver()}));
	}
}