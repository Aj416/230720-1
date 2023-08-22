using System;
using System.Collections.Generic;
using AutoMoqCore;
using FizzWare.NBuilder;
using Moq;
using Tigerspike.Solv.Application.Commands.Fraud;
using Tigerspike.Solv.Services.Fraud.Rules;
using Xunit;

namespace Tigerspike.Solv.Services.Fraud.Tests.Rules
{
	public class ShortInProgressMediumRuleTests
	{
		private IRule GetShortInProgressMediumDetection(int currentResponseTime, List<int> responseTimes)
		{
			DetectFraudCommand message = Builder<DetectFraudCommand>.CreateNew()
						.WithFactory(() => new DetectFraudCommand(currentResponseTime, responseTimes, null, new Guid(), null, true, 4, 1, string.Empty,
							new Guid(), string.Empty, new Guid()))
						.Build();

			var rule = Builder<ShortInProgressMediumRule>
				.CreateNew()
				.WithFactory(() => new ShortInProgressMediumRule(message, new Guid()))
				.Build();

			return rule;
		}

		[Fact]
		public void ShouldValidateShortInProgressMediumRuleWhenResponseTimesAreAvailable()
		{
			var responseTimes = new List<int> { 2, 10 };
			var validDetection = (ShortInProgressMediumRule)GetShortInProgressMediumDetection(7, responseTimes);

			var result = validDetection.IsValid();

			Assert.True(result);
		}

		[Fact]
		public void ShouldValidateShortInProgressMediumRuleWhenCurrentResponseTimeGreaterThanFiveMinutes()
		{
			var responseTimes = new List<int> { 2, 10 };
			var validDetection = (ShortInProgressMediumRule)GetShortInProgressMediumDetection(7, responseTimes);

			var result = validDetection.IsValid();

			Assert.True(result);
		}

		[Fact]
		public void ShouldNotValidateShortInProgressMediumRuleWhenCurrentResponseTimesLessThanFiveMinutes()
		{
			var responseTimes = new List<int> { 2, 4, 1 };
			var validDetection = (ShortInProgressMediumRule)GetShortInProgressMediumDetection(2, responseTimes);

			var result = validDetection.IsValid();

			Assert.False(result);
		}

		[Fact]
		public async void ShouldMatchShortInProgressMediumRuleWhenAtleastFiveResponseTimeLessThanTenMinutes()
		{
			var responseTimes = new List<int> { 5, 6, 8, 8, 3, 7, 12 };
			var validDetection = (ShortInProgressMediumRule)GetShortInProgressMediumDetection(7, responseTimes);

			var result = await validDetection.IsMatchAsync();

			Assert.True(result);
		}

		[Fact]
		public async void ShouldNotMatchShortInProgressMediumRuleWhenAtleastFiveResponseTimeNotLessThanTenMinutes()
		{
			var responseTimes = new List<int> { 6, 1, 3, 4, 8, 7, 5 };
			var validDetection = (ShortInProgressMediumRule)GetShortInProgressMediumDetection(7, responseTimes);

			var result = await validDetection.IsMatchAsync();

			Assert.False(result);
		}
	}
}