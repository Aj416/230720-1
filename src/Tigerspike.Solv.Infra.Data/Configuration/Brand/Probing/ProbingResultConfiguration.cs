using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class ProbingResultConfiguration : IEntityTypeConfiguration<ProbingResult>
	{
		public void Configure(EntityTypeBuilder<ProbingResult> builder)
		{
			// Primary key.
			builder.HasKey(e => new { e.TicketId, e.ProbingQuestionId });

			// Properties
			builder.Property(e => e.TicketId).IsRequired();
			builder.Property(e => e.ProbingQuestionId).IsRequired();
			builder.Property(e => e.ProbingQuestionOptionId);

			// Relations
			builder.HasOne(x => x.ProbingQuestion).WithMany().HasForeignKey(x => x.ProbingQuestionId).OnDelete(DeleteBehavior.Cascade);
			builder.HasOne(x => x.ProbingQuestionOption).WithMany().HasForeignKey(x => x.ProbingQuestionOptionId).OnDelete(DeleteBehavior.Cascade);
		}
	}
}