using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Induction;

namespace Tigerspike.Solv.Infra.Data.Configuration.Induction
{
	public class SectionConfiguration : IEntityTypeConfiguration<Section>
	{
		public void Configure(EntityTypeBuilder<Section> builder)
		{
			builder.ToTable("InductionSection");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();
			builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
			builder.Property(e => e.Enabled).IsRequired();
			builder.Property(e => e.Order).IsRequired();
			builder.HasOne(e => e.Brand).WithMany(x => x.InductionSections).HasForeignKey(c => c.BrandId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}