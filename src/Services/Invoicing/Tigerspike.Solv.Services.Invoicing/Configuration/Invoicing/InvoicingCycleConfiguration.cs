using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Services.Invoicing.Domain;

namespace Tigerspike.Solv.Services.Invoicing.Configuration.Invoicing
{
	public class InvoicingCycleConfiguration : IEntityTypeConfiguration<InvoicingCycle>
	{
		public void Configure(EntityTypeBuilder<InvoicingCycle> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.HasAlternateKey(a => a.To);
			builder.HasAlternateKey(a => a.From);

			// Properties
			builder.Property(e => e.To).IsRequired();
			builder.Property(e => e.From).IsRequired();
			builder.Property(e => e.CreatedDate).IsRequired();
		}
	}
}
