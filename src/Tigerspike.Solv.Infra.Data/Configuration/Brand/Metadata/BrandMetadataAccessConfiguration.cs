using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	/// <summary>
	/// The brand metadata access table configuration
	/// </summary>
	public class BrandMetadataAccessConfiguration : IEntityTypeConfiguration<BrandMetadataAccess>
	{
		/// <summary>
		/// The configuration builder
		/// </summary>
		/// <param name="builder"></param>
		public void Configure(EntityTypeBuilder<BrandMetadataAccess> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			
			builder.Property(e => e.BrandId).IsRequired();
			builder.Property(e => e.Field).IsRequired();
			builder.Property(e => e.Level).IsRequired();
			
			builder.HasOne(x => x.Brand).WithMany(x => x.MetadataAccess).HasForeignKey(x => x.BrandId);
		}
	}
}