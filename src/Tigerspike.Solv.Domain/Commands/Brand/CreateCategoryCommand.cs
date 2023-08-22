using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Brand
{
	public class CreateCategoryCommand : Command
	{
		/// <summary>
		/// Gets or sets the brand id of the category.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Gets or sets list of name and enabled property specific to category.
		/// </summary>
		public List<(string name, bool enabled, int order)> Categories { get; set; }

		/// <summary>
		/// Default cnstructor.
		/// </summary>
		public CreateCategoryCommand() => Categories = new List<(string name, bool enabled, int order)>();

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		public CreateCategoryCommand(Guid brandId, List<(string name, bool enabled, int order)> categories)
		{
			BrandId = brandId;
			Categories = categories;
		}

		public override bool IsValid() => BrandId != Guid.Empty && Categories.Any();
	}
}
