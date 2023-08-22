using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class ClientConfiguration : IEntityTypeConfiguration<Client>
	{
		public void Configure(EntityTypeBuilder<Client> builder)
		{
			// Primary key
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			// Indexes
			builder.HasIndex(e => e.BrandId);

			// Properties
			builder.Property(e => e.BrandId).IsRequired();

			// Relations
			builder.HasOne(e => e.User).WithMany().HasForeignKey(x => x.Id);
			builder.HasOne(e => e.Brand).WithMany().HasForeignKey(x => x.BrandId);
		}
	}
}