using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class TicketEscalationConfigConfiguration : IEntityTypeConfiguration<TicketEscalationConfig>
	{
		public void Configure(EntityTypeBuilder<TicketEscalationConfig> builder)
		{
			// Primary key
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			// Indexes
			builder.HasIndex(e => new { e.BrandId, e.TicketSourceId }).IsUnique();

			// Properties
			builder.Property(e => e.BrandId).IsRequired();
			builder.Property(e => e.TicketSourceId);
			builder.Property(e => e.OpenTimeInSeconds);
			builder.Property(e => e.RejectionCount);
			builder.Property(e => e.AbandonedCount);
			builder.Property(e => e.CustomerMessage);

			// Relations
			builder.HasOne<Brand>().WithMany(x => x.TicketEscalationConfigs).HasForeignKey(x => x.BrandId).IsRequired();
			builder.HasOne<TicketSource>().WithMany().HasForeignKey(x => x.TicketSourceId);
		}
	}
}