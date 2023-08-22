using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Services.Invoicing.Domain;

namespace Tigerspike.Solv.Services.Invoicing.Configuration.Invoicing
{
	public class SequenceConfiguration : IEntityTypeConfiguration<Sequence>
	{
		public void Configure(EntityTypeBuilder<Sequence> builder)
		{
			// PK
			builder.HasKey(e => e.Name);

			// Fields definition
			builder.Property(e => e.Name).HasMaxLength(100);
			builder.Property(e => e.Value).IsRequired();
		}
	}
}
