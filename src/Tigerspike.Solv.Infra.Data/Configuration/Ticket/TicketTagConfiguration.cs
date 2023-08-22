using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	internal class TicketTagConfiguration : IEntityTypeConfiguration<TicketTag>
	{
		public void Configure(EntityTypeBuilder<TicketTag> builder)
		{
			// Primary key.
			builder.HasKey(e => new { e.TagId, e.TicketId, e.Level });

			// Indexes
			builder.HasIndex(e => e.TagId);
			builder.HasIndex(e => e.TicketId);

			// Properties
			builder.Property(e => e.TagId).IsRequired();
			builder.Property(e => e.TicketId).IsRequired();
			builder.Property(e => e.Level).IsRequired();

			// Relations
			builder.HasOne<Ticket>().WithMany(x => x.Tags).HasForeignKey(x => x.TicketId);
			builder.HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId);
		}
	}

}