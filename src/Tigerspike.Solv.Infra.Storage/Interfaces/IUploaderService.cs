using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Tigerspike.Solv.Infra.Storage.Interfaces
{
	public interface IUploaderService
	{
		/// <summary>
		/// Uploads the file to the bucket.
		/// </summary>
		/// <param name="file">The file stream.</param>
		/// <param name="objectKey">The object key with prefix.</param>
		/// <param name="contentType">The content type.</param>
		/// <param name="bucketName">The bucket name.</param>
		/// <param name="publicAccess">Specifies if the file is going to be public or private.</param>
		/// <param name="metaData">The meta data to set on the file.</param>
		/// <returns>The uploaded object key.</returns>
		Task<string> Upload(Stream file, string objectKey, string contentType, string bucketName, bool publicAccess = false, IDictionary<string, object> metaData = null);

		/// <summary>
		/// Uploads the file to the bucket.
		/// </summary>
		/// <param name="file">The file stream.</param>
		/// <param name="objectKey">The object key with prefix.</param>
		/// <param name="bucketName">The bucket name.</param>
		/// <returns>The uploaded object key.</returns>
		Task<string> Upload(Stream file, string objectKey, string bucketName);
	}
}