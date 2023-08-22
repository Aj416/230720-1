using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class AbandonReasonConfiguration : IEntityTypeConfiguration<AbandonReason>
	{
		public void Configure(EntityTypeBuilder<AbandonReason> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.HasIndex(x => new { x.BrandId, x.Name }).IsUnique();

			builder.Property(e => e.BrandId).IsRequired();
			builder.Property(e => e.IsForcedEscalation).IsRequired();
			builder.Property(e => e.IsBlockedAdvocate).IsRequired();
			builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
			builder.Property(e => e.Description);
		}
	}
}