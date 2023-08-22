using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Infra.Data.Configuration.Profiling
{
	public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
	{
		public void Configure(EntityTypeBuilder<Answer> builder)
		{
			builder.ToTable("ProfileAnswer");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.HasOne(e => e.ApplicationAnswer).WithMany(x => x.Answers).HasForeignKey(c => c.ApplicationAnswerId).OnDelete(DeleteBehavior.Cascade);
			builder.HasOne<QuestionOptionCombo>().WithMany().HasForeignKey(a => a.QuestionOptionComboId).OnDelete(DeleteBehavior.Restrict);
			builder.Property(e => e.QuestionOptionId).IsRequired(false);
			builder.Property(e => e.StaticAnswer).HasMaxLength(100);
		}
	}
}