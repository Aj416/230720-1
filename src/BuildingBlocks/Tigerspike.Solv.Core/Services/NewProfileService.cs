using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Mvc;

namespace Tigerspike.Solv.Core.Services
{
	public class NewProfileService : INewProfileService
	{
		private readonly NewProfileOptions _newProfileOptions;

		public NewProfileService(IOptions<NewProfileOptions> newProfileOptions)
		{
			_newProfileOptions = newProfileOptions.Value;
		}

		public bool NewProfileEnable()
		{
			return _newProfileOptions.NewProfile;
		}
	}
}