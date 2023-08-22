using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tigerspike.Solv.Core.UnitOfWork
{
	/// <summary>
	/// Defines the interface(s) for unit of work.
	/// </summary>
	public interface IUnitOfWork
	{
		/// <summary>
		/// Asynchronously saves all changes made in this unit of work to the database.
		/// </summary>
		/// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Executes a raw sql command asynchronously.
		/// </summary>
		/// <param name="sql">The raw sql command.</param>
		/// <param name="parameters">The sql params.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
		Task<int> ExecuteSqlCommandAsync(string sql, IEnumerable<object> parameters, CancellationToken cancellationToken = default );

		/// <summary>
		/// Start db transaction
		/// </summary>
		/// <param name="isolationLevel"></param>
		Task<IDbContextTransaction> BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
	}
}