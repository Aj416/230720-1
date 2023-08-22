
namespace Tigerspike.Solv.Core.Services
{
	public interface IConfiguredJsonSerializer
	{
		/// <summary>
		/// Serializes object to JSON string
		/// </summary>
		/// <returns>JSON representation of passed object</returns>
		string Serialize(object data);

		/// <summary>
		/// Deserializes JSON string into object
		/// </summary>
		/// <returns>Object representation of passed JSON</returns>
		T Deserialize<T>(string json);
	}
}