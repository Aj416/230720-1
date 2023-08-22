using AutoMapper;
using Tigerspike.Solv.Services.Chat.Application.AutoMapper;

namespace Tigerspike.Solv.Chat.Tests.Application.Mappings
{
	public abstract class BaseMappingTest
	{
		protected IMapper SystemUnderTest;

		public BaseMappingTest()
		{
			SystemUnderTest = AutoMapperConfig.GetConfig().CreateMapper();
		}
	}
}