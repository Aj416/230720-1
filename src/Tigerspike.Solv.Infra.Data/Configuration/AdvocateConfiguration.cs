using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class AdvocateConfiguration : IEntityTypeConfiguration<Advocate>
	{
		public void Configure(EntityTypeBuilder<Advocate> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			builder.Property(e => e.InternalAgent).IsRequired();
			builder.Property(e => e.Super).IsRequired();
			builder.Property(e => e.PaymentMethodSetup);
			builder.Property(e => e.PaymentEmailVerified);
			builder.Property(e => e.PaymentAccountId).HasMaxLength(255);
			builder.Property(e => e.ShowBrandNotification);
			builder.Property(e => e.VideoWatched);
			builder.Property(e => e.Practicing);
			builder.Property(e => e.PracticeComplete);
			builder.Property(e => e.Csat).HasColumnType("decimal(3,2)");
			builder.Property(e => e.IdentityApplicantId).HasMaxLength(255);
			builder.Property(e => e.IdentityCheckId).HasMaxLength(255);
			builder.Property(e => e.IdentityCheckResultUrl).HasMaxLength(255);
			builder.Property(e => e.IdentityVerificationStatus);
			builder.Property(e => e.ProfileStatus).IsRequired().HasDefaultValue(AdvocateApplicationProfileStatus.NotStarted);


			// Notice that we made the primary key of the Advocate is the same primary key of the
			// User In other words, the advocate primary key is also a foriegn key to User table.
			builder.HasOne(e => e.User).WithMany().HasForeignKey(c => c.Id);
		}
	}

	internal class AdvocateBlockHistoryConfiguration : IEntityTypeConfiguration<AdvocateBlockHistory>
	{
		public void Configure(EntityTypeBuilder<AdvocateBlockHistory> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Properties
			builder.Property(e => e.AdvocateId);
			builder.Property(e => e.BrandId).IsRequired();
			builder.Property(e => e.CreatedDate).IsRequired();

			builder.HasOne(x => x.Brand).WithMany().HasForeignKey(f => f.BrandId);
			builder.HasOne<Advocate>().WithMany(p => p.BlockHistory).HasForeignKey(d => d.AdvocateId)
				.OnDelete(DeleteBehavior.ClientSetNull);
		}
	}
}