using System;
using FizzWare.NBuilder;
using Tigerspike.Solv.Application.Commands.Fraud;
using Tigerspike.Solv.Services.Fraud.Rules;
using Xunit;

namespace Tigerspike.Solv.Services.Fraud.Tests.Rules
{
    public class QuickCloseRuleTests
    {
        private IRule GetQuickCloseDetection(int? closeTime)
        {
            DetectFraudCommand message = Builder<DetectFraudCommand>.CreateNew()
                        .WithFactory(() => new DetectFraudCommand(null, null, closeTime, new Guid(), null, true, 4, 1, string.Empty,
                            new Guid(), string.Empty, new Guid()))
                        .Build();

            var rule = Builder<QuickCloseRule>
                .CreateNew()
                .WithFactory(() => new QuickCloseRule(message, new Guid()))
                .Build();

            return rule;
        }

        [Fact]
        public void ShouldValidateQuickCloseRuleWhenCloseTimeNotNull()
        {
            var validDetection = (QuickCloseRule)GetQuickCloseDetection(10);

            var result = validDetection.IsValid();

            Assert.True(result);
        }

        [Fact]
        public void ShouldNotValidateQuickCloseRuleWhenCloseTimeNull()
        {
            var validDetection = (QuickCloseRule)GetQuickCloseDetection(null);
            validDetection.Message.CloseTime = null;

            var result = validDetection.IsValid();

            Assert.False(result);
        }

        [Fact]
        public async void ShouldMatchQuickCloseRuleWhenCloseTimeLessThanFiveSeconds()
        {
            var validDetection = (QuickCloseRule)GetQuickCloseDetection(3);

            var result = await validDetection.IsMatchAsync();

            Assert.True(result);
        }

        [Fact]
        public async void ShouldNotMatchQuickCloseRuleWhenCloseTimeGreaterThanFiveSeconds()
        {
            var validDetection = (QuickCloseRule)GetQuickCloseDetection(40);

            var result = await validDetection.IsMatchAsync();

            Assert.False(result);
        }
    }
}