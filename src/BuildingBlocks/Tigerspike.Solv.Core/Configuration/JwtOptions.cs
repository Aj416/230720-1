using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Core.Configuration
{
	public class JwtOptions
	{
		public const string SectionName = "Jwt";

		public string Issuer { get; set; }
		public string Audience  { get; set; }
		public IEnumerable<string> ValidAudiences { get; set; }

		public string Authority  { get; set; }

		public string SecretKey { get; set; }

		public string SdkSecretKey { get; set; }

		public TimeSpan SdkTokenLifeSpan { get; set; } = TimeSpan.FromMinutes(5);

		public TimeSpan CustomerTokenLifeSpan { get; set; } = TimeSpan.FromDays(7);
	}
}
