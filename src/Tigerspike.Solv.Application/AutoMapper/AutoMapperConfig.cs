using AutoMapper;

namespace Tigerspike.Solv.Application.AutoMapper
{
	public class AutoMapperConfig
	{
		//Rule: Add a 'protected' constructor or the 'static' keyword to the class declaration.
		protected AutoMapperConfig()
		{
		}
		public static MapperConfiguration GetConfig() => new MapperConfiguration(x => x.AddMaps(typeof(AutoMapperConfig)));
	}
}