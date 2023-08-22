using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Infra.Data.Context;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	/// <summary>
	/// Represents the default implementation of the <see cref="IUnitOfWork"/> interface.
	/// </summary>
	public class UnitOfWork : IUnitOfWork
	{
		protected SolvDbContext Context { get; }

		public UnitOfWork(SolvDbContext context)
		{
			Context = context;
		}

		/// <inheritdoc />
		public Task<IDbContextTransaction> BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => Context.Database.BeginTransactionAsync(isolationLevel);

		/// <inheritdoc />
		public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
			=> await Context.SaveChangesAsync(cancellationToken);

		/// <inheritdoc />
		public virtual async Task<int> ExecuteSqlCommandAsync(string sql, IEnumerable<object> parameters, CancellationToken cancellationToken = default)
			=> await Context.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
	}
}