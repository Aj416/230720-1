using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class BrandTicketPriceHistoryConfiguration : IEntityTypeConfiguration<BrandTicketPriceHistory>
	{
		public void Configure(EntityTypeBuilder<BrandTicketPriceHistory> builder)
		{
			// primary key
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// fields
			builder.Property(e => e.TicketPrice).PriceColumnType().IsRequired();
			builder.Property(e => e.CreatedDate).IsRequired();

			// foreign keys
			builder.HasOne<Brand>().WithMany().HasForeignKey(f => f.BrandId);
			builder.HasOne<User>().WithMany().HasForeignKey(f => f.UserId);
		}
	}
}