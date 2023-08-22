using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;

namespace Tigerspike.Solv.Core.Extensions
{
	public static class QueryableExtensions
	{
		/// <summary>
		/// Converts the specified source to <see cref="IPagedList{T}"/> by the specified <paramref
		/// name="pageIndex"/> and <paramref name="pageSize"/>.
		/// </summary>
		/// <typeparam name="T">The type of the source.</typeparam>
		/// <param name="source">The source to paging.</param>
		/// <param name="pageIndex">The index of the page.</param>
		/// <param name="pageSize">The size of the page.</param>
		/// <param name="cancellationToken">
		/// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
		/// </param>
		/// <param name="indexFrom">The start index value.</param>
		/// <returns>
		/// An instance of the inherited from <see cref="IPagedList{T}"/> interface.
		/// </returns>
		public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, int indexFrom = 0, CancellationToken cancellationToken = default)
		{
			var count = await source.CountAsync(cancellationToken);
			var items = await source
				.Paginate(pageIndex, pageSize, indexFrom)
				.ToListAsync(cancellationToken);

			return new PagedList<T>(items, pageIndex, pageSize, count, indexFrom);
		}

		/// <summary>
		/// Loads paged include results into memory
		/// </summary>
		/// <typeparam name="T">The type of the source.</typeparam>
		/// <param name="source">The source to paging.</param>
		/// <param name="pageIndex">The index of the page.</param>
		/// <param name="pageSize">The size of the page.</param>
		/// <param name="cancellationToken">
		/// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
		/// </param>
		/// <param name="indexFrom">The start index value.</param>
		public static async Task LoadPagedAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, int indexFrom = 0, CancellationToken cancellationToken = default)
		{
			await source
				.Paginate(pageIndex, pageSize, indexFrom)
				.LoadAsync();
		}

		private static IQueryable<T> Paginate<T>(this IQueryable<T> source, int pageIndex, int pageSize, int indexFrom)
		{
			if (indexFrom > pageIndex)
			{
				throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
			}

			return source
				.Skip((pageIndex - indexFrom) * pageSize)
				.Take(pageSize);
		}

		public static IOrderedQueryable<T> OrderBy<T, TEnum>(this IQueryable<T> query, TEnum sortBy, SortOrder sortOrder) => query.OrderBy($"{sortBy} {sortOrder}");
	}
}