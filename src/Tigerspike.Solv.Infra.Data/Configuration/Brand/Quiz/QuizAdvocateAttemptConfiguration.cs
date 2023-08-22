using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class QuizAdvocateAttemptConfiguration : IEntityTypeConfiguration<QuizAdvocateAttempt>
	{
		public void Configure(EntityTypeBuilder<QuizAdvocateAttempt> builder)
		{
			builder.ToTable("AdvocateQuizAttempt");

			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Properties
			builder.Property(e => e.AdvocateId).IsRequired();
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.Property(e => e.Result).IsRequired();
			builder.Property(e => e.QuizId).IsRequired();

			// Relations
			builder.HasOne<Advocate>().WithMany(a => a.QuizAttempts).HasForeignKey(e => e.AdvocateId).OnDelete(DeleteBehavior.Cascade);
			builder.HasOne<Quiz>().WithMany().HasForeignKey(e => e.QuizId).OnDelete(DeleteBehavior.Cascade);

		}
	}
}