using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Induction;

namespace Tigerspike.Solv.Infra.Data.Configuration.Induction
{
	public class AdvocateSectionItemConfiguration : IEntityTypeConfiguration<AdvocateSectionItem>
	{
		public void Configure(EntityTypeBuilder<AdvocateSectionItem> builder)
		{
			builder.ToTable("InductionAdvocateSectionItem");
			builder.HasKey(e => new {e.AdvocateId, e.SectionItemId});
			builder.HasOne(e => e.SectionItem).WithMany(x => x.AdvocateSectionItems).HasForeignKey(c => c.SectionItemId)
				.OnDelete(DeleteBehavior.Cascade);
			builder.HasOne(e => e.Advocate).WithMany(x => x.AdvocateSectionItems).HasForeignKey(c => c.AdvocateId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}