using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Infra.Data.Configuration.Profiling
{
	public class QuestionDependencyConfiguration : IEntityTypeConfiguration<QuestionDependency>
	{
		public void Configure(EntityTypeBuilder<QuestionDependency> builder)
		{
			builder.ToTable("ProfileQuestionDependency");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.HasOne(e => e.QuestionOption).WithMany(x => x.QuestionDependencies).HasForeignKey(c => c.QuestionOptionId).OnDelete(DeleteBehavior.Cascade);
			builder.HasOne(e => e.Question).WithMany(x => x.QuestionDependencies).HasForeignKey(c => c.QuestionId).OnDelete(DeleteBehavior.Cascade);
		}
	}
}