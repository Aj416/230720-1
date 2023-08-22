using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();
			// Ensure the email is unique.
			builder.HasAlternateKey(a => a.Email);
			// using `builder.HasAlternateKey` makes the column also required (not sure why) while we need this to be unique only.
			builder.HasIndex(e => e.AlternateEmail).IsUnique();

			builder.Property(e => e.Email).HasMaxLength(100);
			builder.Property(e => e.Phone).HasMaxLength(30);
			builder.Property(e => e.FirstName).HasMaxLength(200);
			builder.Property(e => e.LastName).HasMaxLength(200);
			builder.Property(e => e.Enabled);
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.Property(e => e.ModifiedDate).IsRequired();
			builder.Property(e => e.AlternateEmail).HasMaxLength(100);
			builder.Property(e => e.Country).HasMaxLength(5);
			builder.Property(e => e.State).HasMaxLength(2);
		}
	}
}