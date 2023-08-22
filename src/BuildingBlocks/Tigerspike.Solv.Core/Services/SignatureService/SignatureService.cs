using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Tigerspike.Solv.Core.Services
{
	public class SignatureService : ISignatureService
	{
		/// <summary>
		/// Returns HMAC-SHA1 signature of the payload using secret as the key
		/// </summary>
		public string GenerateSha1(string payload, string secret)
		{
			var key = Encoding.UTF8.GetBytes(secret);
			var sha1 = new HMACSHA1(key);
			return Generate(payload, sha1);
		}

		/// <summary>
		/// Returns HMAC-SHA256 signature of the payload using secret as the key
		/// </summary>
		public string GenerateSha256(string payload, string secret)
		{
			var key = Encoding.UTF8.GetBytes(secret);
			var sha256 = new HMACSHA256(key);
			return Generate(payload, sha256);
		}

		private string Generate(string payload, HashAlgorithm cipher)
		{
			var data = Encoding.UTF8.GetBytes(payload);
			var signature = cipher.ComputeHash(data);
			return signature.Aggregate(string.Empty, (s, e) => s + string.Format("{0:x2}", e), s => s);
		}
	}
}