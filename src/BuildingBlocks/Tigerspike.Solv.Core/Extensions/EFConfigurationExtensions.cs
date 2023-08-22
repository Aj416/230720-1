using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tigerspike.Solv.Core.Extensions
{
	public static class EFConfigurationExtensions
	{
		public static PropertyBuilder<TProperty> PriceColumnType<TProperty>(
			this PropertyBuilder<TProperty> propertyBuilder) => propertyBuilder.HasColumnType("decimal(15,2)");

		public static PropertyBuilder<TProperty> PercentageColumnType<TProperty>(
			this PropertyBuilder<TProperty> propertyBuilder) => propertyBuilder.HasColumnType("decimal(6,4)");

		public static PropertyBuilder<TProperty> LongTextColumnType<TProperty>(
			this PropertyBuilder<TProperty> propertyBuilder) => propertyBuilder.HasColumnType("longtext");

		public static PropertyBuilder<TProperty> TinyIntColumnType<TProperty>(
			this PropertyBuilder<TProperty> propertyBuilder) => propertyBuilder.HasColumnType("tinyint");
	}
}
