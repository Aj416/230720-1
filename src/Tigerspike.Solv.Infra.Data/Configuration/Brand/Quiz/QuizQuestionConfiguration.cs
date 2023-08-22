using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class QuizQuestionConfiguration : IEntityTypeConfiguration<QuizQuestion>
	{
		public void Configure(EntityTypeBuilder<QuizQuestion> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			// Properties
			builder.Property(e => e.QuizId).IsRequired();
			builder.Property(e => e.IsMultiChoice).IsRequired();
			builder.Property(e => e.Title).HasMaxLength(300);
			builder.Property(e => e.Order).IsRequired();
			builder.Property(e => e.Enabled).IsRequired();

			// Relations
			builder.HasOne<Quiz>().WithMany(x => x.Questions).HasForeignKey(e => e.QuizId).IsRequired();
		}
	}
}