using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class TicketCategoryConfiguration : IEntityTypeConfiguration<TicketCategory>
	{
		public void Configure(EntityTypeBuilder<TicketCategory> builder)
		{
			// Primary key.
			builder.HasKey(e => new { e.TicketId, e.CategoryId });

			// Indexes
			builder.HasIndex(e => e.TicketId).IsUnique();

			// Properties
			builder.Property(e => e.TicketId).IsRequired();
			builder.Property(e => e.CategoryId).IsRequired();
			builder.Property(e => e.UserId).IsRequired();
			builder.Property(e => e.CreatedDate).IsRequired();

			// Relations
			builder.HasOne<Ticket>().WithOne(x => x.TicketCategory).HasForeignKey<TicketCategory>(x => x.TicketId);
			builder.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId);
		}
	}
}
