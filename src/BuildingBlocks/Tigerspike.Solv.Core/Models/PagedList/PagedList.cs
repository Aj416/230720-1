// Copyright (c) Arch team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Tigerspike.Solv.Core.Models.PagedList
{
	/// <summary>
	/// Provides some help methods for <see cref="IPagedList{T}"/> interface.
	/// </summary>
	public static class PagedList
	{
		/// <summary>
		/// Creates an empty of <see cref="IPagedList{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type for paging</typeparam>
		/// <returns>An empty instance of <see cref="IPagedList{T}"/>.</returns>
		public static IPagedList<T> Empty<T>() => new PagedList<T>();

		/// <summary>
		/// Creates an loop-friendly instance of <see cref="IPagedList{T}"/> (starting with PageIndex = -1).
		/// </summary>
		/// <typeparam name="T">The type for paging</typeparam>
		/// <returns>An loop-friendly instance of <see cref="IPagedList{T}"/>.</returns>
		public static IPagedList<T> ForLoop<T>() => new PagedList<T>() { PageIndex = -1 };

		/// <summary>
		/// Creates a new instance of <see cref="IPagedList{TResult}"/> from source of <see
		/// cref="IPagedList{TSource}"/> instance.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="converter">The converter.</param>
		/// <returns>An instance of <see cref="IPagedList{TResult}"/>.</returns>
		public static IPagedList<TResult> From<TResult, TSource>(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter) => new PagedList<TSource, TResult>(source, converter);

		/// <summary>
		/// Creates a new instance of <see cref="IPagedList{TSource}"/> from an existing page.
		/// The list will be taken as it from already baked result page.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source">The source list that represents a page (not a queryable results)</param>
		/// <param name="pageIndex">The page index of the page that was taken from the original results</param>
		/// <param name="pageSize">The size of the page</param>
		/// <param name="indexFrom"></param>
		/// <returns></returns>
		public static IPagedList<TSource> FromExisting<TSource>(IList<TSource> source, int pageIndex, int pageSize, long totalCount, int indexFrom) => new PagedList<TSource>(source, pageIndex, pageSize, totalCount, indexFrom);
	}

	/// <summary>
	/// Represents the default implementation of the <see cref="IPagedList{T}"/> interface.
	/// </summary>
	/// <typeparam name="T">The type of the data to page</typeparam>
	public class PagedList<T> : IPagedList<T>
	{
		internal PagedList(IList<T> source, int pageIndex, int pageSize, long totalCount, int indexFrom)
		{
			PageIndex = pageIndex;
			TotalCount = totalCount;
			PageSize = pageSize;
			IndexFrom = indexFrom;
			TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
			Items = source;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PagedList{T}"/> class.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="pageIndex">The index of the page.</param>
		/// <param name="pageSize">The size of the page.</param>
		/// <param name="indexFrom">The index from.</param>
		public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom)
		{
			if (indexFrom > pageIndex)
			{
				throw new ArgumentException($"'indexFrom' cannot be larger than 'pageIndex'");
			}

			PageIndex = pageIndex;
			PageSize = pageSize;
			IndexFrom = indexFrom;
			TotalCount = source.Count();
			TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
			Items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PagedList{T}"/> class.
		/// </summary>
		public PagedList() => Items = new T[0];

		/// <summary>
		/// Gets or sets the index of the page.
		/// </summary>
		/// <value>The index of the page.</value>
		public int PageIndex { get; set; }

		/// <summary>
		/// Gets or sets the size of the page.
		/// </summary>
		/// <value>The size of the page.</value>
		public int PageSize { get; set; }

		/// <summary>
		/// Gets or sets the total count.
		/// </summary>
		/// <value>The total count.</value>
		public long TotalCount { get; set; }

		/// <summary>
		/// Gets or sets the total pages.
		/// </summary>
		/// <value>The total pages.</value>
		public int TotalPages { get; set; }

		/// <summary>
		/// Gets or sets the index from.
		/// </summary>
		/// <value>The index from.</value>
		public int IndexFrom { get; set; }

		/// <summary>
		/// Gets or sets the items.
		/// </summary>
		/// <value>The items.</value>
		public IList<T> Items { get; set; }

		/// <summary>
		/// Gets the has previous page.
		/// </summary>
		/// <value>The has previous page.</value>
		public bool HasPreviousPage => PageIndex - IndexFrom > 0;

		/// <summary>
		/// Gets the has next page.
		/// </summary>
		/// <value>The has next page.</value>
		public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;
	}

	/// <summary>
	/// Provides the implementation of the <see cref="IPagedList{T}"/> and converter.
	/// </summary>
	/// <typeparam name="TSource">The type of the source.</typeparam>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	public class PagedList<TSource, TResult> : PagedList<TResult>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PagedList{TSource, TResult}"/> class.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="converter">The converter.</param>
		/// <param name="pageIndex">The index of the page.</param>
		/// <param name="pageSize">The size of the page.</param>
		/// <param name="indexFrom">The index from.</param>
		public PagedList(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int pageIndex, int pageSize, int indexFrom)
			 : base(converter(source), pageIndex, pageSize, indexFrom) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="PagedList{TSource, TResult}"/> class.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="converter">The converter.</param>
		public PagedList(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
		{
			PageIndex = source.PageIndex;
			PageSize = source.PageSize;
			IndexFrom = source.IndexFrom;
			TotalCount = source.TotalCount;
			TotalPages = source.TotalPages;

			Items = new List<TResult>(converter(source.Items));
		}
	}
}
