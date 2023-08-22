
namespace Tigerspike.Solv.Core.Services
{
	public interface ISignatureService
	{
		/// <summary>
		/// Returns signature of the payload using secret as the key with SHA1 hashing algorithm
		/// </summary>
		string GenerateSha1(string payload, string secret);

		/// <summary>
		/// Returns signature of the payload using secret as the key with SHA256 hashing algorithm
		/// </summary>
		string GenerateSha256(string payload, string secret);
	}
}