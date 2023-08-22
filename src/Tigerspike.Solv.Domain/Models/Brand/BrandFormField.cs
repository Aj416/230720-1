using System;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Domain.Models
{
	public class BrandFormField : ICreatedDate, IModifiedDate
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Title { get; set; }
		public int TypeId { get; set; }
		public BrandFormFieldType Type { get; set; }
		public bool IsRequired { get; set; }
		public bool IsKey { get; set; }
		public string Validation { get; set; }
		public string Options { get; set; }
		public string Description { get; set; }
		public string DefaultValue { get; set; }
		public int Order { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ModifiedDate { get; set; }
		public Guid BrandId { get; set; }
		public Brand Brand { get; set; }
		public AccessLevel AccessLevel { get; set; }

	}
}