using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class ProbingQuestionOptionConfiguration : IEntityTypeConfiguration<ProbingQuestionOption>
	{
		public void Configure(EntityTypeBuilder<ProbingQuestionOption> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			// Properties
			builder.Property(e => e.QuestionId).IsRequired();
			builder.Property(e => e.Text).HasMaxLength(300);
			builder.Property(e => e.Order).IsRequired();
			builder.Property(e => e.Action);
			builder.Property(e => e.Value);
			builder.Property(e => e.RedirectAnswer).HasDefaultValue(false);

			// Relations
			builder.HasOne<ProbingQuestion>().WithMany(x => x.Options).HasForeignKey(e => e.QuestionId).IsRequired();
		}
	}
}