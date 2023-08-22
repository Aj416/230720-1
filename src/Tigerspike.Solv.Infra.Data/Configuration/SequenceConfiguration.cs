using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
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