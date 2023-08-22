using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Infra.Data.Configuration.Profiling
{
	public class QuestionTypeConfiguration : IEntityTypeConfiguration<QuestionType>
	{
		public void Configure(EntityTypeBuilder<QuestionType> builder)
		{
			builder.ToTable("ProfileQuestionType");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.Property(e => e.Name).IsRequired().HasMaxLength(300);
		}
	}
}