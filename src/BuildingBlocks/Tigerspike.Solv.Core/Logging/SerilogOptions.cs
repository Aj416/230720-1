using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Core.Logging
{
	public class SerilogOptions
	{
		public ConsoleOptions Console { get; set; }
		public AWSOptions AWS { get; set; }
		public string Level { get; set; }
		public IDictionary<string, string> MinimumLevelOverrides { get; set; }
		public IEnumerable<string> ExcludePaths { get; set; }
		public IEnumerable<string> ExcludeProperties { get; set; }
	}
}