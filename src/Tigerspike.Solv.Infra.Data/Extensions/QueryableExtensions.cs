using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tigerspike.Solv.Infra.Data
{
	public static class QueryableExtensions
	{
		public static Task<int> UpdateAsync<T>(this IQueryable<T> query, Expression<Func<T, T>> updateFactory, bool acquireLock) where T : class
		{
			if (acquireLock)
			{
				return Z.EntityFramework.Plus.BatchUpdateExtensions.UpdateAsync(query, updateFactory, x => x.Executing = (cmd =>
					{
						// get tables touched by the update query - there is no better way to do this unfortunatelly
						var regex = new Regex(@"`?(\w+)`? AS `?(\w+)`?");
						var matches = regex.Matches(cmd.CommandText);

						// prepare lock & unlock statements
						var lockItems = matches.Select(x => $"{x.Value} WRITE");
						var lockStatement = $"LOCK TABLES {string.Join(",", lockItems)}";
						var unlockStatement = "UNLOCK TABLES";

						// decorate the original update query with lock/unlock statements
						cmd.CommandText = string.Join(";", lockStatement, cmd.CommandText, unlockStatement);
					})
				);
			}
			else
			{
				return Z.EntityFramework.Plus.BatchUpdateExtensions.UpdateAsync(query, updateFactory);
			}
		}

	}
}


