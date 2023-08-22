using System;
using System.Collections.Generic;
using System.Linq;

namespace Tigerspike.Solv.Domain.Models.Induction
{
	public class Section
	{
		/// <summary>
		/// Gets or sets the Id of the section.
		/// </summary>
		public Guid Id { get; private set; }

		/// <summary>
		/// Gets or sets the name of the section.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets or sets whether the section is enabled.
		/// </summary>
		public bool Enabled { get; private set; }

		/// <summary>
		/// Gets or sets the brand id of the section.
		/// </summary>
		public Guid BrandId { get; private set; }

		/// <summary>
		/// Gets or sets the brand of the section.
		/// </summary>
		public Brand Brand { get; private set; }

		/// <summary>
		/// Ordering number for the UI
		/// </summary>
		public int Order { get; private set; }

		/// <summary>
		/// Gets or sets the list of section items.
		/// </summary>
		public ICollection<SectionItem> SectionItems { get; private set; }

		public void SetOrder(int order) => Order = order;
		public void SetName(string name) => Name = name;

		public Section() {}

		public Section(Guid id, string name, bool enabled, Guid brandId, int order, ICollection<SectionItem> sectionItems = null)
		{
			Id = id;
			Name = name;
			Enabled = enabled;
			BrandId = brandId;
			Order = order;
			SectionItems = sectionItems;
		}
	}
}