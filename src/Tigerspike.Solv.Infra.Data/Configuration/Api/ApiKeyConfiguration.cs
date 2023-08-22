using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration.Api
{
	public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
	{
		public void Configure(EntityTypeBuilder<ApiKey> builder)
		{
			// Primary key
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.HasAlternateKey(e => e.Key);

			// Indexes
			builder.HasIndex(e => e.BrandId);
			builder.HasIndex(e => e.UserId);
			builder.HasIndex(e => e.ApplicationId).IsUnique();

			// Properties
			builder.Property(e => e.Key).HasMaxLength(36).IsRequired();
			builder.Property(e => e.ApplicationId).HasMaxLength(36);
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.Property(e => e.RevokedDate);

			// Relations
			builder.HasOne<Brand>().WithMany().HasForeignKey(x => x.BrandId).IsRequired();
			builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);
		}
	}
}