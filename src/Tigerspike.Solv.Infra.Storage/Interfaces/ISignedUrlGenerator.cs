using System;

namespace Tigerspike.Solv.Infra.Storage.Interfaces
{
	public interface ISignedUrlGenerator
	{
		/// <summary>
		/// Generates pre-signed urls for the object.
		/// </summary>
		/// <param name="containerName">The bucket name.</param>
		/// <param name="objectKey">The object key.</param>
		/// <param name="duration">The duration before expiry. Not more than 7 days for IAM user.</param>
		/// <returns>The pre-signed url for the object.</returns>
		public string Generate(string containerName, string objectKey, TimeSpan duration);
	}
}