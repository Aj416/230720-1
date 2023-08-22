using System;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Domain.Models
{
	public class ApiKey : ICreatedDate
	{
		/// <summary>
		/// Default api key.
		/// </summary>
		public const string DemoApiKey = "6vT9cqvqrlS3Rk02xuj9IxEuiKt09z6R";

		/// <summary>
		/// Default application id.
		/// </summary>
		public const string DemoApplicationId = "companyone-1";

		/// <summary>
		/// Primary key
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Brand that key is associated with
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Actual key - to pass through authorization header
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// The application id to be used with the sdk token.
		/// </summary>
		public string ApplicationId { get; set; }

		/// <summary>
		/// Owner of the key
		/// </summary>
		public Guid? UserId { get; set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Revoked timestamp (if null, means the key is valid, if not null - revoked)
		/// </summary>
		public DateTime? RevokedDate { get; set; }

		/// <summary>
		/// Constructor for EF
		/// </summary>
		public ApiKey() { }

		public ApiKey(Guid brandId, string m2mKey, string sdkKey)
		{
			BrandId = brandId;
			Key = m2mKey;
			ApplicationId = sdkKey;
		}
	}
}