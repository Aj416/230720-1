using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class BrandFormFieldConfiguration : IEntityTypeConfiguration<BrandFormField>
	{
		public void Configure(EntityTypeBuilder<BrandFormField> builder)
		{
			// Primary key
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Indexes
			builder.HasIndex(e => new { e.Name, e.BrandId }).IsUnique();

			// Fields
			builder.Property(e => e.Name).HasMaxLength(255).IsRequired();
			builder.Property(e => e.Title).HasMaxLength(255).IsRequired();
			builder.Property(e => e.IsRequired).IsRequired();
			builder.Property(e => e.IsKey).IsRequired();
			builder.Property(e => e.Validation).HasMaxLength(255);
			builder.Property(e => e.Options).LongTextColumnType();
			builder.Property(e => e.Description).LongTextColumnType();
			builder.Property(e => e.DefaultValue).HasMaxLength(255);
			builder.Property(e => e.Order).IsRequired();
			builder.Property(e => e.AccessLevel).IsRequired();
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.Property(e => e.ModifiedDate).IsRequired();

			// Foreign keys
			builder.HasOne(x => x.Brand).WithMany(e => e.FormFields).HasForeignKey(x => x.BrandId);
			builder.HasOne(x => x.Type).WithMany().HasForeignKey(x => x.TypeId);
		}
	}
}