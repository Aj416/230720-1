using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	internal class TicketImportConfiguration : IEntityTypeConfiguration<TicketImport>
	{
		public void Configure(EntityTypeBuilder<TicketImport> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Properties
			builder.Property(e => e.FileName).IsRequired();
			builder.Property(e => e.FileType).IsRequired();
			builder.Property(e => e.UploadDate).IsRequired();
			builder.Property(e => e.UserId).IsRequired();
			builder.Property(e => e.ClosedDate).IsRequired();
			builder.Property(e => e.Price);
			builder.Property(e => e.Fee);
			builder.Property(e => e.BrandId);
			builder.Property(e => e.TicketCount);

			// Relations
			builder.HasOne(e => e.User).WithMany().HasForeignKey(f => f.UserId);
			builder.HasMany(e => e.Tickets).WithOne(e => e.TicketImport).HasForeignKey(f => f.TicketImportId);
		}
	}

	internal class TicketImportTagConfiguration : IEntityTypeConfiguration<TicketImportTag>
	{
		public void Configure(EntityTypeBuilder<TicketImportTag> builder)
		{
			// Primary key.
			builder.HasKey(e => new { e.TagId, e.TicketImportId });

			// Indexes
			builder.HasIndex(e => e.TagId);
			builder.HasIndex(e => e.TicketImportId);

			// Properties
			builder.Property(e => e.TagId).IsRequired();
			builder.Property(e => e.TicketImportId).IsRequired();

			// Relations
			builder.HasOne<TicketImport>().WithMany(x => x.Tags).HasForeignKey(x => x.TicketImportId);
			builder.HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId);
		}
	}

	internal class TicketImportFailureConfiguration : IEntityTypeConfiguration<TicketImportFailure>
	{
		public void Configure(EntityTypeBuilder<TicketImportFailure> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Properties
			builder.Property(e => e.TicketImportId).IsRequired();
			builder.Property(e => e.FailureReason).LongTextColumnType();
			builder.Property(e => e.RawInput).LongTextColumnType();

			// Relations
			builder.HasOne<TicketImport>().WithMany(x => x.Failures).HasForeignKey(x => x.TicketImportId);
		}
	}
}