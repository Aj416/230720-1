using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Infra.Bus.Model;

namespace Tigerspike.Solv.Infra.Bus.Repositories
{
	public class ScheduledJobRepository : IScheduledJobRepository
	{
		private readonly IPocoDynamo _db;

		public ScheduledJobRepository(IPocoDynamo db)
		{
			_db = db;
		}

		/// <inheritdoc />
		public ScheduledJob PutItem(ScheduledJob job) => _db.PutItem(job);

		/// <inheritdoc />
		public ScheduledJob GetItem(string jobId)
		{
			return _db
				.FromQuery<ScheduledJob>(x => x.JobId == jobId)
				.Exec()
				.SingleOrDefault();
		}
	}
}