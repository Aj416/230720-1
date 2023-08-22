using System.Threading.Tasks;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface IMaintenanceService
	{
		Task MarkAllEmailsAsVerified();
	}
}