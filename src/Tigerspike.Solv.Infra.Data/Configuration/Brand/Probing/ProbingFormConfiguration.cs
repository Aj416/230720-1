using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class ProbingFormConfiguration : IEntityTypeConfiguration<Domain.Models.ProbingForm>
	{
		public void Configure(EntityTypeBuilder<Domain.Models.ProbingForm> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			// Properties
			builder.Property(e => e.Title).HasMaxLength(300);
		}
	}
}