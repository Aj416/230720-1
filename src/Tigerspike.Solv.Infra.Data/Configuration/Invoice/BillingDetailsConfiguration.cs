using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration.Invoice
{
	public class BillingDetailsConfiguration : IEntityTypeConfiguration<BillingDetails>
	{
		public void Configure(EntityTypeBuilder<BillingDetails> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Properties
			builder.Property(e => e.Name).HasMaxLength(100);
			builder.Property(e => e.Email).HasMaxLength(255);
			builder.Property(e => e.VatNumber).HasMaxLength(30);
			builder.Property(e => e.CompanyNumber).HasMaxLength(30);
			builder.Property(e => e.Address);
			builder.Property(e => e.IsPlatformOwner).IsRequired();
			builder.Property(e => e.CreatedDate).IsRequired();
		}
	}
}