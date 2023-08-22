using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;

namespace Tigerspike.Solv.Core.Services
{
	public class CsvSerializer : ICsvSerializer
	{

		/// <inheritdoc/>
		public async Task<Stream> GetStream<T>(IEnumerable<T> records, bool header = true) => new MemoryStream(await GetData(records, header));

		private async Task<byte[]> GetData<T>(IEnumerable<T> records, bool header = true)
		{
			using (var memoryStream = new MemoryStream())
			using (var streamWriter = new StreamWriter(memoryStream))
			using (var csvWriter = new CsvWriter(streamWriter, System.Threading.Thread.CurrentThread.CurrentCulture))
			{
				if (header)
				{
					csvWriter.WriteHeader<T>();
					await csvWriter.NextRecordAsync();
				}

				await csvWriter.WriteRecordsAsync(records);
				await streamWriter.FlushAsync();
				return memoryStream.ToArray();
			}
		}
	}
}