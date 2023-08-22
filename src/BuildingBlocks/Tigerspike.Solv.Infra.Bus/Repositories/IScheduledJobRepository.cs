using Tigerspike.Solv.Infra.Bus.Model;

namespace Tigerspike.Solv.Infra.Bus.Repositories
{
	public interface IScheduledJobRepository
	{

		/// <summary>
		/// Put a scheduled job item in storage
		/// </summary>
		/// <param name="job">Job item to store</param>
		ScheduledJob PutItem(ScheduledJob job);

		/// <summary>
		/// Gets an scheduled job item base on id
		/// </summary>
		/// <param name="jobId">The job id</param>
		ScheduledJob GetItem(string jobId);
	}
}