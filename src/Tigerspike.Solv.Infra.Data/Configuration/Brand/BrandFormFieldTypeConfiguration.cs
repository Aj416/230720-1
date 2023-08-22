using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class BrandFormFieldTypeConfiguration : IEntityTypeConfiguration<BrandFormFieldType>
	{
		public void Configure(EntityTypeBuilder<BrandFormFieldType> builder)
		{
			// Primary key
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Indexes
			builder.HasIndex(e => e.Name).IsUnique();

			// Fields
			builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
		}
	}
}