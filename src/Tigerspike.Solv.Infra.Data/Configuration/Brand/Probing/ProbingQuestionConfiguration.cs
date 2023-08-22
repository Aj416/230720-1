using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class ProbingQuestionConfiguration : IEntityTypeConfiguration<ProbingQuestion>
	{
		public void Configure(EntityTypeBuilder<ProbingQuestion> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			// Properties
			builder.Property(e => e.ProbingFormId).IsRequired();
			builder.Property(e => e.Text).HasMaxLength(300);
			builder.Property(e => e.Code).HasMaxLength(100);
			builder.Property(e => e.Description).LongTextColumnType();
			builder.Property(e => e.Order).IsRequired();

			// Relations
			builder.HasOne<Domain.Models.ProbingForm>().WithMany(x => x.Questions).HasForeignKey(e => e.ProbingFormId).IsRequired();
		}
	}
}