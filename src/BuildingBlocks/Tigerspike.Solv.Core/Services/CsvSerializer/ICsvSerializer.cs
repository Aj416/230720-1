using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Tigerspike.Solv.Core.Services
{
	public interface ICsvSerializer
	{
		/// <summary>
		/// Get CSV stream for the records
		/// </summary>
		/// <param name="records">Records to be serialized in CSV file</param>
		/// <param name="header">Whether to include header of the model or not</param>
		/// <typeparam name="T"></typeparam>
		Task<Stream> GetStream<T>(IEnumerable<T> records, bool header = true);
	}
}