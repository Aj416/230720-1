using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Infra.Data.Configuration.Profiling
{
	public class AreaConfiguration : IEntityTypeConfiguration<Area>
	{
		public void Configure(EntityTypeBuilder<Area> builder)
		{
			builder.ToTable("ProfileArea");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.Property(e => e.Order).IsRequired();
			builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
			builder.Property(e => e.Enabled);
		}
	}
}