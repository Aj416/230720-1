using AutoMoqCore;
using Tigerspike.Solv.Application.Consumers;

namespace Tigerspike.Solv.Application.Tests.Validators
{
	public abstract class ImportConsumerTestsBase
	{
		protected readonly AutoMoqer Mocker = new AutoMoqer();
		protected ImportConsumer SystemUnderTest => Mocker.Create<ImportConsumer>();

	}
}