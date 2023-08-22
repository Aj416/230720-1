using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface IPaymentRepository : IRepository<Payment>
	{

		/// <summary>
		/// Gets date of first transaction between the parties
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="advocateId">The advocate id</param>
		Task<DateTime?> GetFirstDate(Guid brandId, Guid advocateId);

		/// <summary>
		/// Gets date of first transaction made by brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		Task<DateTime?> GetFirstDate(Guid brandId);

		/// <summary>
		/// Get the date of the last payment for the specified advocate.
		/// </summary>
		Task<DateTime?> GetLastPaymentDate(Guid advocateId);
	}
}