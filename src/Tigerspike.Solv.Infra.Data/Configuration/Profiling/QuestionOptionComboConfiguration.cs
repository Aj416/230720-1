using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Infra.Data.Configuration.Profiling
{
	public class QuestionOptionComboConfiguration : IEntityTypeConfiguration<QuestionOptionCombo>
	{
		public void Configure(EntityTypeBuilder<QuestionOptionCombo> builder)
		{
			builder.ToTable("ProfileQuestionOptionCombo");

			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Properties
			builder.Property(e => e.ComboOptionTitle).IsRequired();
			builder.Property(e => e.ComboOptionType).IsRequired();
			builder.Property(e => e.QuestionId).IsRequired();
			builder.Property(e => e.Order).IsRequired();
			builder.Property(e => e.Enabled).IsRequired().HasDefaultValue(true);
			builder.Property(e => e.OptionsPerRow).IsRequired();

			// Relations
			builder.HasOne<Question>().WithMany(q => q.QuestionOptionCombos).HasForeignKey(qoc => qoc.QuestionId);
		}
	}
}
