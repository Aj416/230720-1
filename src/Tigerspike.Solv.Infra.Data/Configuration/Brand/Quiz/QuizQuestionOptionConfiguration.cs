using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class QuizQuestionOptionConfiguration : IEntityTypeConfiguration<QuizQuestionOption>
	{
		public void Configure(EntityTypeBuilder<QuizQuestionOption> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			// Properties
			builder.Property(e => e.QuestionId).IsRequired();
			builder.Property(e => e.Correct).IsRequired();
			builder.Property(e => e.Text).LongTextColumnType();
			builder.Property(e => e.Order).IsRequired();
			builder.Property(e => e.Enabled).IsRequired();

			// Relations
			builder.HasOne<QuizQuestion>().WithMany(x => x.Options).HasForeignKey(e => e.QuestionId).IsRequired();
		}
	}
}