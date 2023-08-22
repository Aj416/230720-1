using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Infra.Storage.Interfaces;

namespace Tigerspike.Solv.Infra.Storage.S3
{
	public class S3UploaderService : IUploaderService
	{
		private static IAmazonS3 _client;
		private readonly ILogger<S3UploaderService> _logger;

		public S3UploaderService(IAmazonS3 client, ILogger<S3UploaderService> logger)
		{
			_client = client;
			_logger = logger;
		}

		/// <inheritdoc />
		public Task<string> Upload(Stream file, string objectKey, string bucketName) => Upload(file, objectKey, string.Empty, bucketName);

		/// <inheritdoc />
		public async Task<string> Upload(Stream file, string objectKey, string contentType, string bucketName, bool publicAccess = false, IDictionary<string, object> metaData = null)
		{
			try
			{
				using (var ms = new MemoryStream())
				using (var fileTransferUtility = new TransferUtility(_client))
				{
					await file.CopyToAsync(ms);

					var fileTransferRequest = new TransferUtilityUploadRequest
					{
						BucketName = bucketName,
						Key = objectKey,
						InputStream = ms,
						ContentType = contentType,
						ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
						AutoCloseStream = false
					};

					if (metaData?.Count > 0)
					{
						metaData.ToList().ForEach(md => fileTransferRequest.Metadata.Add(md.Key, md.Value.ToString()));
					}

					await fileTransferUtility.UploadAsync(fileTransferRequest);

					if (publicAccess)
					{
						await _client.MakeObjectPublicAsync(bucketName, fileTransferRequest.Key, enable: true);
					}

					return fileTransferRequest.Key;
				}
			}
			catch (AmazonS3Exception ex)
			{
				_logger.LogError("Upload to S3 failed", ex);
				throw;
			}
		}

	}
}