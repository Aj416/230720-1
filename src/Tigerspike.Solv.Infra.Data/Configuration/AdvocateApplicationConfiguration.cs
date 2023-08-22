using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class AdvocateApplicationConfiguration : IEntityTypeConfiguration<AdvocateApplication>
	{
		public void Configure(EntityTypeBuilder<AdvocateApplication> builder)
		{
			builder.ToTable("AdvocateApplication");
			builder.HasIndex(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();
			builder.Property(e => e.Country).HasMaxLength(64);
			builder.Property(e => e.State).HasMaxLength(20);
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.Property(e => e.DeletionHash).HasMaxLength(64);
			builder.Property(e => e.Email).HasMaxLength(255);
			builder.Property(e => e.Token).IsRequired(false);
			builder.Property(e => e.Phone).HasMaxLength(30).HasDefaultValue(null);
			builder.Property(e => e.Source).HasMaxLength(50);
			builder.Property(e => e.FirstName).HasMaxLength(200);
			builder.Property(e => e.LastName).HasMaxLength(200);
			builder.Property(e => e.InvitationEmailSent).HasDefaultValue(false);
			builder.Property(e => e.ResponseEmailSent).HasDefaultValue(false);
			builder.Property(e => e.CompletedEmailSent);
			builder.Property(e => e.ApplicationStatus).HasDefaultValue(AdvocateApplicationStatus.New);
			builder.Property(e => e.InvitationDate).IsRequired(false);
			builder.Property(e => e.LastInvitationDate).IsRequired(false);
			builder.Property(e => e.InternalAgent).IsRequired();
			builder.Property(e => e.Address);
			builder.Property(e => e.City);
			builder.Property(e => e.ZipCode);
		}
	}
}