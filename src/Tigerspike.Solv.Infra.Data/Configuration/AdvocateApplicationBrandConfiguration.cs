using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class AdvocateApplicationBrandConfiguration : IEntityTypeConfiguration<AdvocateApplicationBrand>
	{
		public void Configure(EntityTypeBuilder<AdvocateApplicationBrand> builder)
		{
			builder.HasKey(e => new {e.AdvocateApplicationId, e.BrandId});
			builder.HasOne(x => x.Brand).WithMany(e => e.AdvocateApplications).HasForeignKey(x => x.BrandId)
				.IsRequired();
			builder.HasOne(x => x.AdvocateApplication).WithMany(x => x.BrandAssignments)
				.HasForeignKey(x => x.AdvocateApplicationId).IsRequired();
		}
	}
}