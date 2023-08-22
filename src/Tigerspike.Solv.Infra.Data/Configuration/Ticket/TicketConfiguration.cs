using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	internal class TicketConfiguration : IEntityTypeConfiguration<Ticket>
	{
		public void Configure(EntityTypeBuilder<Ticket> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			// Indexes
			builder.HasIndex(e => e.AdvocateId);
			builder.HasIndex(e => e.BrandId);
			builder.HasIndex(e => e.Status);
			builder.HasIndex(e => e.Question);

			// Unique indexes
			builder.HasIndex(e => e.Number);
			builder.HasIndex(e => new { e.BrandId, e.ReferenceId }).IsUnique();
			builder.HasIndex(e => e.ThreadId);

			// Other
			builder.Property(a => a.RowVersion)
				.IsRowVersion();

			// Properties
			builder.Property(e => e.Number).IsRequired();
			builder.Property(e => e.AdvocateId);
			builder.Property(e => e.BrandId).IsRequired();
			builder.Property(e => e.Level).IsRequired().HasDefaultValue(TicketLevel.Regular);
			builder.Property(e => e.Complexity).TinyIntColumnType();
			builder.Property(e => e.Csat).TinyIntColumnType();
			builder.Property(e => e.CsatDate);
			builder.Property(e => e.Nps).TinyIntColumnType();
			builder.Property(e => e.NpsDate);
			builder.Property(e => e.Status);
			builder.Property(e => e.AbandonedCount).TinyIntColumnType();
			builder.Property(e => e.RejectionCount).TinyIntColumnType();
			builder.Property(e => e.Price).PriceColumnType();
			builder.Property(e => e.Fee).PriceColumnType();
			builder.Property(e => e.Question).HasMaxLength(255).IsRequired();
			builder.Property(e => e.IsPractice);
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.Property(e => e.ModifiedDate).IsRequired();
			builder.Property(e => e.AssignedDate);
			builder.Property(e => e.FirstAssignedDate);
			builder.Property(e => e.FirstMessageDate);
			builder.Property(e => e.LastAdvocateMessageDate);
			builder.Property(e => e.LastCustomerMessageDate);
			builder.Property(e => e.CurrentCustomerQueryDate);
			builder.Property(e => e.ClosedDate);
			builder.Property(e => e.ReservationExpiryDate);
			builder.Property(e => e.ClientInvoiceId);
			builder.Property(e => e.AdvocateInvoiceId);
			builder.Property(e => e.ReferenceId).HasMaxLength(50);
			builder.Property(e => e.ThreadId).HasMaxLength(50);
			builder.Property(e => e.SourceId);
			builder.Property(e => e.EscalatedDate);
			builder.Property(e => e.EscalationReason);
			builder.Property(e => e.TransportType).TinyIntColumnType().IsRequired();
			builder.Property(e => e.ClosedBy).TinyIntColumnType();
			builder.Property(e => e.CorrectlyDiagnosed);
			builder.Property(e => e.ChaserEmails);
			builder.Property(e => e.SuperSolverFirstMessageDate);
			builder.Property(e => e.ReturningCustomerState).TinyIntColumnType().IsRequired();
			builder.Property(e => e.FraudRisks);
			builder.Property(e => e.TicketImportId);
			builder.Property(e => e.SolverTotalResponseTimeInSeconds);
			builder.Property(e => e.SolverMaxResponseTimeInSeconds);
			builder.Property(e => e.SolverResponseCount);
			builder.Property(e => e.SolverMessageCount);
			builder.Property(e => e.CustomerMessageCount);
			builder.Property(e => e.SposEmailSent);
			builder.Property(e => e.ValidTransfer);
			builder.Property(e => e.AdditionalFeedBack).LongTextColumnType();
			builder.Property(e => e.Culture).HasMaxLength(10);
			builder.Property(e => e.RepeatedInL1);
			builder.Property(e => e.RepeatedInL2);
			builder.Property(e => e.TagStatus);
			builder.Property(e => e.Ready).IsRequired();

			// Relations
			builder.HasOne(e => e.Customer).WithMany().IsRequired();
			builder.HasOne(e => e.EscalatedSolver).WithMany().HasForeignKey(f => f.EscalatedById);
			builder.HasOne<ClientInvoice>().WithMany().HasForeignKey(f => f.ClientInvoiceId);
			builder.HasOne<AdvocateInvoice>().WithMany().HasForeignKey(f => f.AdvocateInvoiceId);
			builder.HasOne(e => e.Advocate).WithMany().HasForeignKey(f => f.AdvocateId);
			builder.HasOne(e => e.Brand).WithMany().HasForeignKey(f => f.BrandId);
			builder.HasMany(e => e.StatusHistory).WithOne().HasForeignKey(sh => sh.TicketId);
			builder.HasMany(e => e.RejectionHistory).WithOne().HasForeignKey(sh => sh.TicketId);
			builder.HasMany(e => e.Metadata).WithOne().HasForeignKey(f => f.TicketId);
			builder.HasOne(e => e.Source).WithMany().HasForeignKey(f => f.SourceId);
			builder.HasMany(e => e.AbandonHistory).WithOne().HasForeignKey(sh => sh.TicketId);
			builder.HasMany(e => e.ProbingAnswers).WithOne().HasForeignKey(sh => sh.TicketId);
		}
	}

	internal class TicketRejectionReasonConfiguration : IEntityTypeConfiguration<TicketRejectionReason>
	{
		public void Configure(EntityTypeBuilder<TicketRejectionReason> builder)
		{
			builder.HasKey(nameof(TicketRejectionReason.TicketRejectionHistoryId),
				nameof(TicketRejectionReason.RejectionReasonId));

			builder.Property(e => e.RejectionReasonId);
			builder.Property(e => e.TicketRejectionHistoryId);

			builder.HasOne<TicketRejectionHistory>().WithMany(m => m.Reasons).HasForeignKey(t => t.TicketRejectionHistoryId);
			builder.HasOne(x => x.RejectionReason).WithMany().HasForeignKey(t => t.RejectionReasonId);
		}
	}

	internal class TicketStatusHistoryConfiguration : IEntityTypeConfiguration<TicketStatusHistory>
	{
		public void Configure(EntityTypeBuilder<TicketStatusHistory> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Properties
			builder.Property(e => e.TicketId).IsRequired();
			builder.Property(e => e.Status).IsRequired();
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.Property(e => e.Level).IsRequired();
			builder.Property(e => e.AdvocateId);
			builder.HasOne(x => x.Advocate).WithMany().HasForeignKey(f => f.AdvocateId);
		}
	}

	internal class TicketRejectionHistoryConfiguration : IEntityTypeConfiguration<TicketRejectionHistory>
	{
		public void Configure(EntityTypeBuilder<TicketRejectionHistory> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			// Properties
			builder.Property(e => e.TicketId).IsRequired();
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.HasMany(e => e.Reasons).WithOne().HasForeignKey(trr => trr.TicketRejectionHistoryId);
		}
	}

	internal class TicketSourceConfiguration : IEntityTypeConfiguration<TicketSource>
	{
		public void Configure(EntityTypeBuilder<TicketSource> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.HasIndex(e => e.Name).IsUnique();
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Properties
			builder.Property(e => e.Name).IsRequired().HasMaxLength(30);
		}
	}

	internal class TicketMetadataItemConfiguration : IEntityTypeConfiguration<TicketMetadataItem>
	{
		public void Configure(EntityTypeBuilder<TicketMetadataItem> builder)
		{
			// Primary key.
			builder.HasKey(e => new { e.TicketId, e.Key });
			builder.Property(e => e.Value).LongTextColumnType();
			builder.Property(e => e.Order);
			builder.Property(e => e.BrandFormFieldId);

			builder.HasOne<BrandFormField>().WithMany().HasForeignKey(t => t.BrandFormFieldId);
		}
	}

	internal class TicketAbandonReasonConfiguration : IEntityTypeConfiguration<TicketAbandonReason>
	{
		public void Configure(EntityTypeBuilder<TicketAbandonReason> builder)
		{
			builder.HasKey(nameof(TicketAbandonReason.TicketAbandonHistoryId),
				nameof(TicketAbandonReason.AbandonReasonId));

			builder.Property(e => e.AbandonReasonId);
			builder.Property(e => e.TicketAbandonHistoryId);

			builder.HasOne<TicketAbandonHistory>().WithMany(m => m.Reasons).HasForeignKey(t => t.TicketAbandonHistoryId);
			builder.HasOne(x => x.AbandonReason).WithMany().HasForeignKey(t => t.AbandonReasonId);
		}
	}

	internal class TicketAbandonHistoryConfiguration : IEntityTypeConfiguration<TicketAbandonHistory>
	{
		public void Configure(EntityTypeBuilder<TicketAbandonHistory> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			// Properties
			builder.Property(e => e.TicketId).IsRequired();
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.HasMany(e => e.Reasons).WithOne().HasForeignKey(trr => trr.TicketAbandonHistoryId);
		}
	}
}