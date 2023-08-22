using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models
{
	public class CreateCategoryModel
	{
		/// <summary>
		/// Gets or sets list of category.
		/// </summary>
		public IList<CategoryListModel> Categories { get; set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public CreateCategoryModel() => Categories = new List<CategoryListModel>();
	}

	public class CategoryListModel
	{
		/// <summary>
		/// Gets or sets the name of the category
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets whether the category is enabled
		/// </summary>
		public bool? Enabled { get; set; }

		/// <summary>
		/// Gets or sets order in which Category to be displayed.
		/// </summary>
		public int Order { get; set; }
	}
}
