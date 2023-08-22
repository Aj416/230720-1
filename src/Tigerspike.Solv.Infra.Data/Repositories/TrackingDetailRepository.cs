using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class TrackingDetailRepository : Repository<TrackingDetail>, ITrackingDetailRepository
	{
		public TrackingDetailRepository(SolvDbContext context) : base(context) { }
	}
}
