using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration.Invoice
{
	public class AdvocateInvoiceConfiguration : IEntityTypeConfiguration<AdvocateInvoice>
	{
		public void Configure(EntityTypeBuilder<AdvocateInvoice> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Indexes
			builder.HasIndex(e => e.AdvocateId);
			builder.HasIndex(e => e.InvoicingCycleId);
			builder.HasIndex(c => new { c.AdvocateId, c.ReferenceNumber }).IsUnique();
			builder.HasIndex(c => new { c.AdvocateId, c.Sequence }).IsUnique();

			// Properties
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.Property(e => e.ModifiedDate).IsRequired();
			builder.Property(e => e.InvoicingCycleId).IsRequired();
			builder.Property(e => e.ReferenceNumber).IsRequired();
			builder.Property(e => e.Total).PriceColumnType().IsRequired();
			builder.Property(e => e.Sequence);
			builder.Property(e => e.Status);
			builder.Property(e => e.AmountPaid).PriceColumnType();
			builder.Property(e => e.PaymentFailureDate);

			// Relations
			builder.HasOne<Advocate>().WithMany().HasForeignKey(f => f.AdvocateId).IsRequired();
			builder.HasOne(x => x.Advocate).WithMany().HasForeignKey(f => f.AdvocateId).IsRequired();
			builder.HasOne(x => x.InvoicingCycle).WithMany().HasForeignKey(f => f.InvoicingCycleId).IsRequired();
			builder.HasOne(x => x.PlatformBillingDetails).WithMany().HasForeignKey(x => x.PlatformBillingDetailsId).OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasMany(x => x.Tickets).WithOne().HasForeignKey(f => f.AdvocateInvoiceId).OnDelete(DeleteBehavior.Restrict);
			builder.HasMany(m => m.LineItems).WithOne(x => x.AdvocateInvoice).HasForeignKey(f => f.AdvocateInvoiceId).OnDelete(DeleteBehavior.Cascade).IsRequired();
		}
	}
}