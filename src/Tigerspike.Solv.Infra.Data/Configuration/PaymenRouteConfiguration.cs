using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class PaymenRouteConfiguration : IEntityTypeConfiguration<PaymentRoute>
	{
		public void Configure(EntityTypeBuilder<PaymentRoute> builder)
		{
			// Primary key.
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			// Properties
			builder.Property(e => e.Name).IsRequired().HasMaxLength(30);
		}
	}
}