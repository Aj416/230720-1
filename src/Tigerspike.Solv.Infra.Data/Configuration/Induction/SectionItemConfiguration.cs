using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Induction;

namespace Tigerspike.Solv.Infra.Data.Configuration.Induction
{
	public class SectionItemConfiguration : IEntityTypeConfiguration<SectionItem>
	{
		public void Configure(EntityTypeBuilder<SectionItem> builder)
		{
			builder.ToTable("InductionSectionItem");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();
			builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
			builder.Property(e => e.Source).IsRequired();
			builder.Property(e => e.Enabled).IsRequired();
			builder.Property(e => e.Order).IsRequired();
			builder.HasOne(e => e.Section).WithMany(x => x.SectionItems).HasForeignKey(c => c.SectionId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}