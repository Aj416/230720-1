using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class TagConfiguration : IEntityTypeConfiguration<Tag>
	{
		public void Configure(EntityTypeBuilder<Tag> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.HasIndex(x => new { x.BrandId, x.Name }).IsUnique();

			builder.Property(e => e.BrandId).IsRequired();
			builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
			builder.Property(e => e.Action);
			builder.Property(e => e.Level);
			builder.Property(e => e.Enabled).IsRequired().HasDefaultValue(true);
			builder.Property(e => e.ParentTagId);
			builder.Property(e => e.DiagnosisEnabled);
			builder.Property(e => e.Description);
			builder.Property(e => e.L1PostClosureDisable).IsRequired().HasDefaultValue(false);
			builder.Property(e => e.L2PostClosureDisable).IsRequired().HasDefaultValue(false);

			// Relations
			builder.HasMany(e => e.SubTags).WithOne(e => e.ParentTag).HasForeignKey(e => e.ParentTagId);
		}
	}
}