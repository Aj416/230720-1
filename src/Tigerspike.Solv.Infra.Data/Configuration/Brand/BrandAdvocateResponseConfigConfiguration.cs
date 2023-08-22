using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class BrandAdvocateResponseConfigConfiguration : IEntityTypeConfiguration<BrandAdvocateResponseConfig>
	{
		public void Configure(EntityTypeBuilder<BrandAdvocateResponseConfig> builder)
		{
			// Primary key
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Fields
			builder.Property(e => e.BrandId);
			builder.Property(e => e.Type).TinyIntColumnType().IsRequired();
			builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
			builder.Property(e => e.Content).LongTextColumnType();
			builder.Property(e => e.DelayInSeconds);
			builder.Property(e => e.RelevantTo);
			builder.Property(e => e.ChatActionId);
			builder.Property(e => e.Priority).IsRequired();
			builder.Property(e => e.AbandonedCount);
			builder.Property(e => e.IsAutoAbandoned);
			builder.Property(e => e.EscalationReason);
			builder.Property(e => e.AuthorUserType);
			builder.Property(e => e.StatusOnPosting);

			// Foreign keys
			builder.HasOne<Brand>().WithMany().HasForeignKey(x => x.BrandId);
		}
	}
}