using System;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Infra.Storage.Interfaces;

namespace Tigerspike.Solv.Infra.Storage.S3
{
	public class S3PreSignedUrlGenerator : ISignedUrlGenerator
	{
		private readonly IAmazonS3 _s3Client;
		private readonly ILogger<S3PreSignedUrlGenerator> _logger;

		public S3PreSignedUrlGenerator(IAmazonS3 s3Client, ILogger<S3PreSignedUrlGenerator> logger)
		{
			_s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public string Generate(string containerName, string objectKey, TimeSpan duration)
		{
			if (duration > new TimeSpan(7, 0, 0, 0))
			{
				throw new ArgumentOutOfRangeException(nameof(duration), "Should not be more than 7 days for IAM user.");
			}

			var urlString = "";

			try
			{
				var request = new GetPreSignedUrlRequest
				{
					BucketName = containerName,
					Key = objectKey,
					Expires = DateTime.UtcNow.Add(duration),
					Verb = HttpVerb.GET
				};

				urlString = _s3Client.GetPreSignedURL(request);
			}
			catch (AmazonS3Exception ex)
			{
				_logger.LogError(ex, "Error encountered on server. Message:'{0}' when writing an object", ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unknown encountered on server. Message:'{0}' when writing an object", ex.Message);
			}

			return urlString;
		}
	}
}