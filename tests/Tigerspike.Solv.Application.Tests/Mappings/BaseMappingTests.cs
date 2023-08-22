using AutoMapper;
using Tigerspike.Solv.Application.AutoMapper;

namespace Tigerspike.Solv.Application.Tests.Mappings
{
	public abstract class BaseMappingTests
	{
		protected IMapper SystemUnderTest;

		public BaseMappingTests()
		{
			SystemUnderTest = AutoMapperConfig.GetConfig().CreateMapper();
		}
	}
}