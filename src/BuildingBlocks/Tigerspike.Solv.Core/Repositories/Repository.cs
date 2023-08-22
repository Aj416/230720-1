using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Models.PagedList;

namespace Tigerspike.Solv.Core.Repositories
{
	/// <summary>
	/// Represents a default generic repository implements the <see cref="IRepository{TEntity}"/>
	/// interface.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <summary>
	/// Represents a default generic repository implements the <see cref="IRepository{TEntity}"/> interface.
	/// </summary>
	public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
	{
		protected readonly DbContext DbContext;
		protected readonly DbSet<TEntity> DbSet;

		/// <summary>
		/// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		public Repository(DbContext dbContext)
		{
			DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			DbSet = DbContext.Set<TEntity>();
		}

		/// <inheritdoc />
		public TContextType GetContext<TContextType>()
			where TContextType : class =>
			DbContext as TContextType;

		/// <inheritdoc />
		public virtual void ChangeTable(string table)
		{
			if (DbContext.Model.FindEntityType(typeof(TEntity)) is IConventionEntityType relational)
			{
				relational.SetTableName(table);
			}
		}

		/// <inheritdoc />
		public IQueryable<TEntity> Queryable()
		{
			return DbSet;
		}


		/// <inheritdoc />
		public IQueryable<TEntity> GetAll(
			Expression<Func<TEntity, bool>> predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = true, bool ignoreQueryFilters = false)
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (ignoreQueryFilters)
			{
				query = query.IgnoreQueryFilters();
			}

			if (orderBy != null)
			{
				return orderBy(query);
			}
			else
			{
				return query;
			}
		}

		/// <inheritdoc />
		public virtual IPagedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate = null,
												Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
												Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
												int pageIndex = 0,
												int pageSize = 20,
												bool disableTracking = true,
												bool ignoreQueryFilters = false)
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (ignoreQueryFilters)
			{
				query = query.IgnoreQueryFilters();
			}

			if (orderBy != null)
			{
				return orderBy(query).ToPagedList(pageIndex, pageSize);
			}
			else
			{
				return query.ToPagedList(pageIndex, pageSize);
			}
		}


		/// <inheritdoc />
		public virtual Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate = null,
														   Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
														   Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
														   int pageIndex = 0,
														   int pageSize = 20,
														   bool disableTracking = true,
														   CancellationToken cancellationToken = default,
														   bool ignoreQueryFilters = false)
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (ignoreQueryFilters)
			{
				query = query.IgnoreQueryFilters();
			}

			if (orderBy != null)
			{
				return orderBy(query).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
			}
			else
			{
				return query.ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
			}
		}


		/// <inheritdoc />
		public virtual IPagedList<TResult> GetPagedList<TResult>(Expression<Func<TEntity, TResult>> selector,
														 Expression<Func<TEntity, bool>> predicate = null,
														 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
														 Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
														 int pageIndex = 0,
														 int pageSize = 20,
														 bool disableTracking = true,
														 bool ignoreQueryFilters = false)
			where TResult : class
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (ignoreQueryFilters)
			{
				query = query.IgnoreQueryFilters();
			}

			if (orderBy != null)
			{
				return orderBy(query).Select(selector).ToPagedList(pageIndex, pageSize);
			}
			else
			{
				return query.Select(selector).ToPagedList(pageIndex, pageSize);
			}
		}


		/// <inheritdoc />
		public virtual Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
																	Expression<Func<TEntity, bool>> predicate = null,
																	Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
																	Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
																	int pageIndex = 0,
																	int pageSize = 20,
																	bool disableTracking = true,
																	CancellationToken cancellationToken = default,
																	bool ignoreQueryFilters = false)
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (ignoreQueryFilters)
			{
				query = query.IgnoreQueryFilters();
			}

			if (orderBy != null)
			{
				return orderBy(query).Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
			}
			else
			{
				return query.Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
			}
		}

		/// <inheritdoc/>
		public async Task<IPagedList<TResult>> GetPagedListAsync<TResult>(
					IMapper mapper,
					Expression<Func<TEntity, bool>> predicate = null,
					Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
					int pageIndex = 0,
					int pageSize = 20,
					CancellationToken cancellationToken = default)
		{
			IQueryable<TEntity> query = DbSet;

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			var orderedQuery = orderBy != null ? orderBy(query) : query.OrderBy(x => true);

			return await orderedQuery
				.ProjectTo<TResult>(mapper.ConfigurationProvider)
				.ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
		}

		/// <inheritdoc />
		public virtual TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null,
										 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
										 Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
										 bool disableTracking = true,
										 bool ignoreQueryFilters = false)
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (ignoreQueryFilters)
			{
				query = query.IgnoreQueryFilters();
			}

			if (orderBy != null)
			{
				return orderBy(query).FirstOrDefault();
			}
			else
			{
				return query.FirstOrDefault();
			}
		}


		/// <inheritdoc />
		public virtual async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
			bool disableTracking = true,
			bool ignoreQueryFilters = false)
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (ignoreQueryFilters)
			{
				query = query.IgnoreQueryFilters();
			}

			if (orderBy != null)
			{
				return await orderBy(query).FirstOrDefaultAsync();
			}
			else
			{
				return await query.FirstOrDefaultAsync();
			}
		}

		/// <inheritdoc/>
		public virtual async Task<TResult> GetFirstOrDefaultAsync<TResult>(
					IMapper mapper,
					Expression<Func<TEntity, bool>> predicate = null,
					Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
		{
			IQueryable<TEntity> query = DbSet;

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			var orderedQuery = orderBy != null ? orderBy(query) : query.OrderBy(x => true);

			return await orderedQuery
				.ProjectTo<TResult>(mapper.ConfigurationProvider)
				.FirstOrDefaultAsync();
		}


		/// <inheritdoc />
		public virtual TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
												  Expression<Func<TEntity, bool>> predicate = null,
												  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
												  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
												  bool disableTracking = true,
												  bool ignoreQueryFilters = false)
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (ignoreQueryFilters)
			{
				query = query.IgnoreQueryFilters();
			}

			if (orderBy != null)
			{
				return orderBy(query).Select(selector).FirstOrDefault();
			}
			else
			{
				return query.Select(selector).FirstOrDefault();
			}
		}

		/// <inheritdoc />
		public virtual async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
												  Expression<Func<TEntity, bool>> predicate = null,
												  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
												  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
												  bool disableTracking = true, bool ignoreQueryFilters = false)
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (ignoreQueryFilters)
			{
				query = query.IgnoreQueryFilters();
			}

			if (orderBy != null)
			{
				return await orderBy(query).Select(selector).FirstOrDefaultAsync();
			}
			else
			{
				return await query.Select(selector).FirstOrDefaultAsync();
			}
		}

		/// <inheritdoc/>
		public async Task<TEntity> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
			bool disableTracking = true)
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (orderBy != null)
			{
				return await orderBy(query).SingleOrDefaultAsync();
			}
			else
			{
				return await query.SingleOrDefaultAsync();
			}
		}

		/// <inheritdoc/>
		public async Task<TResult> GetSingleOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
			Expression<Func<TEntity, bool>> predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
			bool disableTracking = true)
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (orderBy != null)
			{
				return await orderBy(query).Select(selector).SingleOrDefaultAsync();
			}
			else
			{
				return await query.Select(selector).SingleOrDefaultAsync();
			}
		}

		/// <inheritdoc />
		public virtual IQueryable<TEntity> FromSql(string sql, params object[] parameters) => DbSet.FromSqlRaw(sql, parameters);


		/// <inheritdoc />
		public virtual TEntity Find(params object[] keyValues) => DbSet.Find(keyValues);


		/// <inheritdoc />
		public virtual ValueTask<TEntity> FindAsync(params object[] keyValues) => DbSet.FindAsync(keyValues);


		/// <inheritdoc />
		public virtual ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken) => DbSet.FindAsync(keyValues, cancellationToken);


		/// <inheritdoc />
		public virtual int Count(Expression<Func<TEntity, bool>> predicate = null)
		{
			if (predicate == null)
			{
				return DbSet.Count();
			}
			else
			{
				return DbSet.Count(predicate);
			}
		}


		/// <inheritdoc />
		public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
		{
			if (predicate == null)
			{
				return await DbSet.CountAsync();
			}
			else
			{
				return await DbSet.CountAsync(predicate);
			}
		}


		/// <inheritdoc />
		public virtual long LongCount(Expression<Func<TEntity, bool>> predicate = null)
		{
			if (predicate == null)
			{
				return DbSet.LongCount();
			}
			else
			{
				return DbSet.LongCount(predicate);
			}
		}


		/// <inheritdoc />
		public virtual async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null)
		{
			if (predicate == null)
			{
				return await DbSet.LongCountAsync();
			}
			else
			{
				return await DbSet.LongCountAsync(predicate);
			}
		}


		/// <inheritdoc />
		public virtual T Max<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
		{
			if (predicate == null)
				return DbSet.Max(selector);
			else
				return DbSet.Where(predicate).Max(selector);
		}


		/// <inheritdoc />
		public virtual async Task<T> MaxAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
		{
			if (predicate == null)
				return await DbSet.MaxAsync(selector);
			else
				return await DbSet.Where(predicate).MaxAsync(selector);
		}


		/// <inheritdoc />
		public virtual T Min<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
		{
			if (predicate == null)
				return DbSet.Min(selector);
			else
				return DbSet.Where(predicate).Min(selector);
		}


		/// <inheritdoc />
		public virtual async Task<T> MinAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
		{
			if (predicate == null)
				return await DbSet.MinAsync(selector);
			else
				return await DbSet.Where(predicate).MinAsync(selector);
		}


		/// <inheritdoc />
		public virtual decimal Average(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
		{
			if (predicate == null)
				return DbSet.Average(selector);
			else
				return DbSet.Where(predicate).Average(selector);
		}


		/// <inheritdoc />
		public virtual async Task<decimal> AverageAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
		{
			if (predicate == null)
				return await DbSet.AverageAsync(selector);
			else
				return await DbSet.Where(predicate).AverageAsync(selector);
		}


		/// <inheritdoc />
		public virtual decimal Sum(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
		{
			if (predicate == null)
				return DbSet.Sum(selector);
			else
				return DbSet.Where(predicate).Sum(selector);
		}


		/// <inheritdoc />
		public virtual async Task<decimal> SumAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
		{
			if (predicate == null)
				return await DbSet.SumAsync(selector);
			else
				return await DbSet.Where(predicate).SumAsync(selector);
		}


		/// <inheritdoc />
		public bool Exists(Expression<Func<TEntity, bool>> selector = null)
		{
			if (selector == null)
			{
				return DbSet.Any();
			}
			else
			{
				return DbSet.Any(selector);
			}
		}

		/// <inheritdoc />
		public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null)
		{
			if (selector == null)
			{
				return await DbSet.AnyAsync();
			}
			else
			{
				return await DbSet.AnyAsync(selector);
			}
		}

		/// <inheritdoc />
		public virtual TEntity Insert(TEntity entity)
		{
			return DbSet.Add(entity).Entity;
		}


		/// <inheritdoc />
		public virtual void Insert(params TEntity[] entities) => DbSet.AddRange(entities);


		/// <inheritdoc />
		public virtual void Insert(IEnumerable<TEntity> entities) => DbSet.AddRange(entities);


		/// <inheritdoc />
		public virtual ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
		{
			return DbSet.AddAsync(entity, cancellationToken);

			// Shadow properties?
			//var property = _dbContext.Entry(entity).Property("Created");
			//if (property != null) {
			//property.CurrentValue = DateTime.Now;
			//}
		}


		/// <inheritdoc />
		public virtual Task InsertAsync(params TEntity[] entities) => DbSet.AddRangeAsync(entities);


		/// <inheritdoc />
		public virtual Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => DbSet.AddRangeAsync(entities, cancellationToken);


		/// <inheritdoc />
		public virtual void Update(TEntity entity) => _ = DbSet.Update(entity);

		/// <summary>
		/// Updates the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public virtual void UpdateAsync(TEntity entity) => _ = DbSet.Update(entity);


		/// <inheritdoc />
		public virtual void Update(params TEntity[] entities) => DbSet.UpdateRange(entities);


		/// <inheritdoc />
		public virtual void Update(IEnumerable<TEntity> entities) => DbSet.UpdateRange(entities);

		
		/// <inheritdoc />
		public virtual void Delete(TEntity entity) => DbSet.Remove(entity);


		/// <inheritdoc />
		public virtual void Delete(object id)
		{
			// using a stub entity to mark for deletion
			var typeInfo = typeof(TEntity).GetTypeInfo();
			var key = DbContext.Model.FindEntityType(typeInfo).FindPrimaryKey().Properties.FirstOrDefault();
			var property = typeInfo.GetProperty(key?.Name);
			if (property != null)
			{
				var entity = Activator.CreateInstance<TEntity>();
				property.SetValue(entity, id);
				DbContext.Entry(entity).State = EntityState.Deleted;
			}
			else
			{
				var entity = DbSet.Find(id);
				if (entity != null)
				{
					Delete(entity);
				}
			}
		}


		/// <inheritdoc />
		public virtual void Delete(params TEntity[] entities) => DbSet.RemoveRange(entities);


		/// <inheritdoc />
		public virtual void Delete(IEnumerable<TEntity> entities) => DbSet.RemoveRange(entities);


		/// <inheritdoc />
		public async Task<IList<TEntity>> GetAllAsync()
		{
			return await DbSet.ToListAsync();
		}


		/// <inheritdoc />
		public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
			bool disableTracking = true, bool ignoreQueryFilters = false)
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (ignoreQueryFilters)
			{
				query = query.IgnoreQueryFilters();
			}

			if (orderBy != null)
			{
				return await orderBy(query).ToListAsync();
			}
			else
			{
				return await query.ToListAsync();
			}
		}

		public async Task<IList<TResult>> GetAllAsync<TResult>(IMapper mapper, Expression<Func<TEntity, bool>> predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
			bool disableTracking = true, bool ignoreQueryFilters = false)
		{
			IQueryable<TEntity> query = DbSet;

			if (disableTracking)
			{
				query = query.AsNoTracking();
			}

			if (include != null)
			{
				query = include(query);
			}

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (ignoreQueryFilters)
			{
				query = query.IgnoreQueryFilters();
			}

			if (orderBy != null)
			{
				return await orderBy(query)
					.ProjectTo<TResult>(mapper.ConfigurationProvider)
					.ToListAsync();
			}
			else
			{
				return await query
                    .ProjectTo<TResult>(mapper.ConfigurationProvider)
                    .ToListAsync();
			}
		}


		/// <inheritdoc />
		public void ChangeEntityState(TEntity entity, EntityState state)
		{
			DbContext.Entry(entity).State = state;
		}

		/// <inheritdoc/>
		public Task<int> SaveChangesAsync() => DbContext.SaveChangesAsync();
	}
}