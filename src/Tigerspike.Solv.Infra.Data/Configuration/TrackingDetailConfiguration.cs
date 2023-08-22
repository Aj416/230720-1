using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class TrackingDetailConfiguration : IEntityTypeConfiguration<TrackingDetail>
	{
		public void Configure(EntityTypeBuilder<TrackingDetail> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Properties
			builder.Property(e => e.UserId).IsRequired();
			builder.Property(e => e.TicketId).IsRequired();
			builder.Property(e => e.IpAddress);
			builder.Property(e => e.UserAgent);
			builder.Property(e => e.Event).IsRequired();
			builder.Property(e => e.CreatedDate).IsRequired();

			// Relations
			builder.HasOne<User>(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
			builder.HasOne<Ticket>().WithMany(e => e.TrackingHistory).HasForeignKey(e => e.TicketId).OnDelete(DeleteBehavior.Cascade);
		}
	}
}