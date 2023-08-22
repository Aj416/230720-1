using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tigerspike.Solv.Core.Extensions
{
	public static class EnumerableExtensions
	{
		private static readonly Type GuidType = typeof(Guid);

		public static string Concatenate<TValue>(this IEnumerable<TValue> list, string separator = ",", string defaultResult = "")
		{
			if (list != null)
			{
				var valueType = typeof(TValue);
				var transformed = list.Select(x => valueType == GuidType ? $"'{x.ToString()}'" : x.ToStringInvariant());
				var count = transformed.Count();
				return count switch
				{
					0 => defaultResult,
					1 => transformed.First(),
					_ => transformed.Aggregate((x, y) => x + separator + y),
				};
			}

			return defaultResult;
		}

		public static async Task<bool> AllAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> predicate)
		{
			if (source == null) { throw new ArgumentNullException(nameof(source)); }

			if (predicate == null) { throw new ArgumentNullException(nameof(predicate)); }

			foreach (var item in source)
			{
				var result = await predicate(item);
				if (result == false)
				{
					return false;
				}
			}

			return true;
		}

		public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property) => items.GroupBy(property).Select(x => x.First());

		public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> items, IEnumerable<T> other, Func<T, TKey> getKey)
		{
			return from item in items
				join otherItem in other on getKey(item)
				equals getKey(otherItem) into tempItems
				from temp in tempItems.DefaultIfEmpty()
				where ReferenceEquals(null, temp) || temp.Equals(default(T))
				select item;
		}

		public static IEnumerable<T> Intersect<T, TKey>(this IEnumerable<T> items, IEnumerable<T> other, Func<T, TKey> getKey)
		{
			return from item in items
				join otherItem in other on getKey(item)
				equals getKey(otherItem) into tempItems
				from temp in tempItems.DefaultIfEmpty()
				where ReferenceEquals(null, temp) == false
				select item;
		}

		public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> sequence)
		{
			return sequence ?? Enumerable.Empty<T>();
		}

	}
}