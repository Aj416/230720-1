using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Domain.Models.Induction
{
	public class SectionItem
	{
		/// <summary>
		/// Gets or sets the id of the section item.
		/// </summary>
		public Guid Id { get; private set; }

		/// <summary>
		/// Gets or sets the name of the section item.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets or sets the source of the section item i.e. https://google.com.
		/// </summary>
		public string Source { get; private set; }

		/// <summary>
		/// Gets or sets whether the section item is enabled.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the section id of the section item.
		/// </summary>
		public Guid SectionId { get; private set; }

		/// <summary>
		/// Gets or sets the section of the section item.
		/// </summary>
		public Section Section { get; private set; }

		/// <summary>
		/// Ordering number for the UI
		/// </summary>
		public int Order { get; private set; }

		/// <summary>
		/// Gets or sets the list of answers for the section items.
		/// </summary>
		public List<AdvocateSectionItem> AdvocateSectionItems { get; private set; }

		public void SetOrder(int order) => Order = order;
		public void SetName(string name) => Name = name;
		public void SetSource(string source) => Source = source;
		public void SetSection(Section section) => Section = section;

		public SectionItem(Guid id, string name, string source, bool enabled, int order)
		{
			Id = id;
			Name = name;
			Source = source;
			Enabled = enabled;
			Order = order;
		}
	}
}