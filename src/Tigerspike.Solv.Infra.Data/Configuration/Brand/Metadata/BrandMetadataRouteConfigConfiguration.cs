using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	/// <summary>
	/// The brand metadata route config table configuration
	/// </summary>
	public class BrandMetadataRouteConfigConfiguration : IEntityTypeConfiguration<BrandMetadataRoutingConfig>
	{
		/// <summary>
		/// The configuration builder
		/// </summary>
		/// <param name="builder"></param>
		public void Configure(EntityTypeBuilder<BrandMetadataRoutingConfig> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			
			builder.Property(e => e.BrandId).IsRequired();
			builder.Property(e => e.Field).IsRequired();
			builder.Property(e => e.Value).IsRequired();
			builder.Property(e => e.RouteTo).IsRequired();
			
			builder.HasOne(x => x.Brand).WithOne(x => x.RoutingConfig).HasForeignKey<BrandMetadataRoutingConfig>(x => x.BrandId);
		}
	}
}