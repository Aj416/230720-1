using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration.Invoice
{
	public class ClientInvoiceConfiguration : IEntityTypeConfiguration<ClientInvoice>
	{
		public void Configure(EntityTypeBuilder<ClientInvoice> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Indexes
			builder.HasIndex(e => e.BrandId);
			builder.HasIndex(e => e.InvoicingCycleId);
			builder.HasIndex(e => e.ReferenceNumber).IsUnique();
			builder.HasIndex(e => e.Sequence).IsUnique();

			// Properties
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.Property(e => e.BrandId).IsRequired();
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.Property(e => e.ModifiedDate).IsRequired();
			builder.Property(e => e.TicketsCount).IsRequired();
			builder.Property(e => e.ReferenceNumber).IsRequired();
			builder.Property(e => e.Fee).PriceColumnType().IsRequired();
			builder.Property(e => e.Price).PriceColumnType().IsRequired();
			builder.Property(e => e.InvoiceTotal).PriceColumnType().IsRequired();
			builder.Property(e => e.PaymentTotal).PriceColumnType().IsRequired();
			builder.Property(e => e.VatRate).PercentageColumnType();
			builder.Property(e => e.VatAmount).PriceColumnType();
			builder.Property(e => e.Subtotal).PriceColumnType().IsRequired();
			builder.Property(e => e.Sequence);
			builder.Property(e => e.Status);
			builder.Property(e => e.AmountPaid).PriceColumnType();
			builder.Property(e => e.PaymentFailureDate);

			// Relations
			builder.HasOne(x => x.InvoicingCycle).WithMany().HasForeignKey(f => f.InvoicingCycleId).IsRequired();
			builder.HasOne(x => x.BrandBillingDetails).WithMany().HasForeignKey(x => x.BrandBillingDetailsId).OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne(x => x.PlatformBillingDetails).WithMany().HasForeignKey(x => x.PlatformBillingDetailsId).OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne(x => x.Brand).WithMany().HasForeignKey(e => e.BrandId).IsRequired();
			builder.HasMany(x => x.Tickets).WithOne().HasForeignKey(f => f.ClientInvoiceId).OnDelete(DeleteBehavior.Restrict);
		}
	}
}