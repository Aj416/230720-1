using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Infra.Data.Configuration.Profiling
{
	public class QuestionConfiguration : IEntityTypeConfiguration<Question>
	{
		public void Configure(EntityTypeBuilder<Question> builder)
		{
			builder.ToTable("ProfileQuestion");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.HasOne(e => e.Area).WithMany(x => x.Questions).HasForeignKey(c => c.AreaId).OnDelete(DeleteBehavior.Cascade);
			builder.HasOne(e => e.QuestionType).WithMany(x => x.Questions).HasForeignKey(c => c.QuestionTypeId).OnDelete(DeleteBehavior.Cascade);
			builder.Property(e => e.Order).IsRequired();
			builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
			builder.Property(e => e.SubTitle).IsRequired().HasMaxLength(300);
			builder.Property(e => e.OptionsPerRow);
			builder.Property(e => e.Header);
		}
	}
}