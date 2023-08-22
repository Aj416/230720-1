
namespace Tigerspike.Solv.Core.Services
{
	/// <summary>
	///
	/// </summary>
	public interface IJsonContentProvider
	{
		/// <summary>
		/// Creates JsonContent from object
		/// </summary>
		/// <returns>JSON representation of passed object in strongly typed HttpContent</returns>
		JsonContent CreateJsonContent(object data);
	}
}