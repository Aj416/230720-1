using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class AdvocateBrandConfiguration : IEntityTypeConfiguration<AdvocateBrand>
	{
		public void Configure(EntityTypeBuilder<AdvocateBrand> builder)
		{
			builder.HasKey(e => new { e.AdvocateId, e.BrandId });
			builder.Property(e => e.AdvocateId).ValueGeneratedNever();
			builder.Property(e => e.BrandId).ValueGeneratedNever();
			builder.Property(e => e.Authorized).IsRequired();
			builder.Property(e => e.AgreementAccepted).IsRequired();
			builder.Property(e => e.ContractAccepted).IsRequired();
			builder.Property(e => e.Csat).HasColumnType("decimal(3,2)");
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.Property(e => e.Inducted).IsRequired();
			builder.Property(e => e.Enabled).IsRequired();
			builder.Property(e => e.ModifiedDate).IsRequired();
			builder.Property(e => e.Blocked).HasDefaultValue(false);
			builder.Property(e => e.AuthorizedDate);
			builder.Property(e => e.InductedDate);
			builder.Property(e => e.ContractAcceptedDate);
			builder.Property(e => e.GuidelineAgreed).HasDefaultValue(false);


			builder.HasOne<Advocate>().WithMany(p => p.Brands).HasForeignKey(d => d.AdvocateId)
				.OnDelete(DeleteBehavior.ClientSetNull);
			builder.HasOne(d => d.Brand).WithMany(p => p.Advocates).HasForeignKey(d => d.BrandId)
				.OnDelete(DeleteBehavior.ClientSetNull);
			builder.HasOne(d => d.User).WithMany().HasForeignKey(d => d.AdvocateId);
		}
	}
}