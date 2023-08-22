using System;

namespace Tigerspike.Solv.Core.Services
{
	public class JsonContentProvider : IJsonContentProvider
	{
		private readonly IConfiguredJsonSerializer _serializer;
		public JsonContentProvider(IConfiguredJsonSerializer serializer)
		{
			_serializer = serializer;
		}

		/// <inheritdoc/>
		public JsonContent CreateJsonContent(object data)
		{
			var json = _serializer.Serialize(data);
			return new JsonContent(json);
		}
	}
}