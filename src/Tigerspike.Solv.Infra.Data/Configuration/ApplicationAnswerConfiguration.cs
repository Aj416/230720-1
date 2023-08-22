using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class ApplicationAnswerConfiguration : IEntityTypeConfiguration<ApplicationAnswer>
	{
		public void Configure(EntityTypeBuilder<ApplicationAnswer> builder)
		{
			builder.ToTable("ProfileApplicationAnswer");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.HasOne(e => e.AdvocateApplication).WithMany(x => x.ApplicationAnswers).HasForeignKey(c => c.AdvocateApplicationId).OnDelete(DeleteBehavior.Cascade);
			builder.HasOne(e => e.Question).WithMany(x => x.ApplicationAnswers).HasForeignKey(c => c.QuestionId).OnDelete(DeleteBehavior.Cascade);
		}
	}
}