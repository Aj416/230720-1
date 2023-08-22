using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class CategoryConfiguration : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);

			// Indexes
			builder.HasIndex(x => new { x.BrandId, x.Name }).IsUnique();

			// Properties
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.Property(e => e.BrandId).IsRequired();
			builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
			builder.Property(e => e.Enabled).IsRequired().HasDefaultValue(true);
			builder.Property(e => e.Order).IsRequired();
		}
	}
}
