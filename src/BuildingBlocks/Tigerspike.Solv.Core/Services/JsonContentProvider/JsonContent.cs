using System.Net.Http;
using System.Text;

namespace Tigerspike.Solv.Core.Services
{
	public class JsonContent : StringContent
	{
		public JsonContent(string json) : base(json, Encoding.UTF8, "application/json") { }
	}
}