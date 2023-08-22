using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Infra.Data.Configuration.Profiling
{
	public class QuestionOptionDependencyConfiguration : IEntityTypeConfiguration<QuestionOptionDependency>
	{
		public void Configure(EntityTypeBuilder<QuestionOptionDependency> builder)
		{
			builder.ToTable("ProfileQuestionOptionDependency");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.HasOne(e => e.QuestionOption).WithMany(x => x.QuestionOptionDependencies).HasForeignKey(c => c.QuestionOptionId).OnDelete(DeleteBehavior.Cascade);
		}
	}
}