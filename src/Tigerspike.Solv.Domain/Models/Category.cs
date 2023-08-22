using System;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Category specific data
	/// </summary>
	public class Category
	{
		/// <summary>
		/// Gets or sets the Id of the category
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the brand id of the category.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Gets or sets the name of the category
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets whether the category is enabled
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// Gets or sets order in which category is displayed.
		/// </summary>
		public int Order { get; set; }

		/// <summary>
		/// Default category constructor
		/// </summary>
		public Category() { }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		public Category(Guid brandId, string name, bool enabled, int order)
		{
			BrandId = brandId;
			Name = name;
			Enabled = enabled;
			Order = order;
		}

	}
}
