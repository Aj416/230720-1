using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class BrandProfileConfiguration : IEntityTypeConfiguration<ProfileBrand>
	{
		public void Configure(EntityTypeBuilder<ProfileBrand> builder)
		{
			builder.ToTable("ProfileBrands");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.Property(e => e.BrandName).HasMaxLength(100);
		}
	}
}