using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class BrandNotificationConfigConfiguration : IEntityTypeConfiguration<BrandNotificationConfig>
	{
		public void Configure(EntityTypeBuilder<BrandNotificationConfig> builder)
		{
			// Primary key
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Fields
			builder.Property(e => e.Type).TinyIntColumnType().IsRequired();
			builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
			builder.Property(e => e.DeliverAfterSeconds).IsRequired();
			builder.Property(e => e.Subject).LongTextColumnType();
			builder.Property(e => e.Header).LongTextColumnType();
			builder.Property(e => e.Body).LongTextColumnType();

			// Foreign keys
			builder.HasOne(x => x.Brand).WithMany(e => e.NotificationConfig).HasForeignKey(x => x.BrandId);
		}
	}
}