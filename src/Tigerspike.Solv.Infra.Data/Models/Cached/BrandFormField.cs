using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Infra.Data.Models.Cached
{
	public class BrandFormField
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string Title { get; set; }

		public int TypeId { get; set; }

		public string Type { get; set; }

		public bool IsRequired { get; set; }
		public bool IsKey { get; set; }

		public string Validation { get; set; }

		public string Options { get; set; }
		public string Description { get; set; }
		public string DefaultValue { get; set; }

		public int Order { get; set; }
		public AccessLevel AccessLevel { get; set; }
	}
}