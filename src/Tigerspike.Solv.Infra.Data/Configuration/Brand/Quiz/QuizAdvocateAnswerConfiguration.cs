using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class QuizAdvocateAnswerConfiguration : IEntityTypeConfiguration<QuizAdvocateAnswer>
	{
		public void Configure(EntityTypeBuilder<QuizAdvocateAnswer> builder)
		{
			builder.ToTable("AdvocateQuizAnswer");

			// Primary key.
			builder.HasKey(e => new { e.QuizAdvocateAttemptId, e.QuizQuestionOptionId });

			// Relations
			builder.HasOne<QuizAdvocateAttempt>().WithMany(x => x.Answers).HasForeignKey(e => e.QuizAdvocateAttemptId).OnDelete(DeleteBehavior.Cascade);
			builder.HasOne<QuizQuestionOption>().WithMany().HasForeignKey(c => c.QuizQuestionOptionId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}