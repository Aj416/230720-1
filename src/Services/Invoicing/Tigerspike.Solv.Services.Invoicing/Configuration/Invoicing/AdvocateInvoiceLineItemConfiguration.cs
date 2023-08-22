using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Services.Invoicing.Domain;

namespace Tigerspike.Solv.Services.Invoicing.Configuration.Invoicing
{
	public class AdvocateInvoiceLineItemConfiguration : IEntityTypeConfiguration<AdvocateInvoiceLineItem>
	{
		public void Configure(EntityTypeBuilder<AdvocateInvoiceLineItem> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Indexes
			builder.HasIndex(e => e.AdvocateInvoiceId);
			builder.HasIndex(e => e.BrandId);

			// Properties
			builder.Property(e => e.BrandId).IsRequired();
			builder.Property(e => e.BrandName).IsRequired();
			builder.Property(e => e.TicketsCount).IsRequired();
			builder.Property(e => e.Amount).PriceColumnType().IsRequired();
			builder.Property(e => e.AdvocateInvoiceId).IsRequired();
		}
	}
}
