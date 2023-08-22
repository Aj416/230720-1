using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
	{
		public void Configure(EntityTypeBuilder<Quiz> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			// Properties
			builder.Property(e => e.Title).HasMaxLength(300);
			builder.Property(e => e.FailureMessage).LongTextColumnType();
			builder.Property(e => e.SuccessMessage).LongTextColumnType();
			builder.Property(e => e.Description).LongTextColumnType();
			builder.Property(e => e.AllowedMistakes).IsRequired();
		}
	}
}