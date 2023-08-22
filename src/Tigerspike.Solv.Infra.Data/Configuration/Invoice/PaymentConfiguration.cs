using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration.Invoice
{
	public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
	{
		public void Configure(EntityTypeBuilder<Payment> builder)
		{
			// Primary key
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.HasAlternateKey(e => e.ReferenceNumber);

			// Indexes
			builder.HasIndex(e => e.ClientInvoiceId);

			// Properties
			builder.Property(e => e.Amount).PriceColumnType().IsRequired();
			builder.Property(e => e.ClientInvoiceId);
			builder.Property(e => e.ReferenceNumber).HasMaxLength(255).IsRequired();
			builder.Property(e => e.AdvocateInvoiceLineItemId);

			// Relations
			builder.HasOne(x => x.AdvocateInvoiceLineItem).WithOne(x => x.Payment);
			builder.HasOne<ClientInvoice>().WithMany().HasForeignKey(x => x.ClientInvoiceId);
		}
	}
}
