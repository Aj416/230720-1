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
    public class ShortInProgressHighRuleTests
    {
        private readonly AutoMoqer Mocker = new AutoMoqer();

        private IRule GetShortInProgressHighDetection(int currentResponseTime, List<int> responseTimes)
        {
            DetectFraudCommand message = Builder<DetectFraudCommand>.CreateNew()
                        .WithFactory(() => new DetectFraudCommand(currentResponseTime, responseTimes, null, new Guid(), null, true, 4, 1, string.Empty,
                            new Guid(), string.Empty, new Guid()))
                        .Build();

            var rule = Builder<ShortInProgressHighRule>
                .CreateNew()
                .WithFactory(() => new ShortInProgressHighRule(message, new Guid()))
                .Build();

            return rule;
        }

        [Fact]
        public void ShouldValidateShortInProgressHighRuleWhenResponseTimesAreAvailable()
        {
            var responseTimes = new List<int> { 2, 10 };
            var validDetection = (ShortInProgressHighRule)GetShortInProgressHighDetection(2, responseTimes);

            var result = validDetection.IsValid();

            Assert.True(result);
        }

        [Fact]
        public void ShouldNotValidateShortInProgressHighRuleWhenResponseTimesNotAvailable()
        {
            var validDetection = (ShortInProgressHighRule)GetShortInProgressHighDetection(2, null);

            var result = validDetection.IsValid();

            Assert.False(result);
        }

        [Fact]
        public async void ShouldMatchShortInProgressHighRuleWhenAtleastFiveResponseTimeLessThanFiveMinutes()
        {
            var responseTimes = new List<int> { 2, 1, 3, 4, 3, 7 };
            var validDetection = (ShortInProgressHighRule)GetShortInProgressHighDetection(2, responseTimes);

            var result = await validDetection.IsMatchAsync();

            Assert.True(result);
        }

        [Fact]
        public async void ShouldNotMatchShortInProgressHighRuleWhenAtleastFiveResponseTimeNotLessThanFiveMinutes()
        {
            var responseTimes = new List<int> { 2, 1, 3, 14, 3, 7 };
            var validDetection = (ShortInProgressHighRule)GetShortInProgressHighDetection(2, responseTimes);

            var result = await validDetection.IsMatchAsync();

            Assert.False(result);
        }
    }
}