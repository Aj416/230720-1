using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Infra.Data.Configuration.Profiling
{
	public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
	{
		public void Configure(EntityTypeBuilder<QuestionOption> builder)
		{
			builder.ToTable("ProfileQuestionOption");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.HasOne(e => e.Question).WithMany(x => x.QuestionOptions).HasForeignKey(c => c.QuestionId).OnDelete(DeleteBehavior.Cascade);
			builder.HasOne<QuestionOptionCombo>().WithMany(qoc => qoc.ComboOptions).HasForeignKey(qo => qo.QuestionOptionComboId);
			builder.Property(e => e.Order).IsRequired();
			builder.Property(e => e.Text).IsRequired().HasMaxLength(300);
			builder.Property(e => e.SubText).IsRequired().HasMaxLength(300);
			builder.Property(e => e.BusinessValue);
		}
	}
}