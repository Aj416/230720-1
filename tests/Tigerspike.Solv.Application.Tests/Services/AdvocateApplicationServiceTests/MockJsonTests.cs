using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateApplicationServiceTests
{
	public class MockJsonTests : BaseClass
	{
		[Fact]
		public void GenerateMockQuestionJsonShouldReturnNotEmpty()
		{
			// Act
			var result = JsonConvert.SerializeObject(QuestionsViewModel,
				new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

			// Assert
			Assert.NotEmpty(result);
		}

		[Fact]
		public void GenerateMockAnswerJsonShouldReturnNotEmpty()
		{
			// Act
			var result = JsonConvert.SerializeObject(AnswerViewModel,
				new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

			// Assert
			Assert.NotEmpty(result);
		}
	}
}