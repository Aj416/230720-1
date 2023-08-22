using System;
using AutoMoqCore;
using FizzWare.NBuilder;
using Moq;
using Tigerspike.Solv.Application.Commands.Fraud;
using Tigerspike.Solv.Services.Fraud.Rules;
using Xunit;

namespace Tigerspike.Solv.Services.Fraud.Tests.Rules
{
    public class SuspiciousCsatRuleTests
    {
        private readonly AutoMoqer Mocker = new AutoMoqer();

        private IRule GetSuspiciousCsatDetection(int? csat)
        {
            DetectFraudCommand message = Builder<DetectFraudCommand>.CreateNew()
                        .WithFactory(() => new DetectFraudCommand(null, null, null, new Guid(), csat, true, 4, 1, string.Empty,
                            new Guid(), string.Empty, new Guid()))
                        .Build();

            var rule = Builder<SuspiciousCsatRule>
                .CreateNew()
                .WithFactory(() => new SuspiciousCsatRule(message, new Guid()))
                .Build();

            return rule;
        }

        [Fact]
        public void ShouldValidateSuspiciousCsatRuleWhenCsatNotNull()
        {
            var validDetection = (SuspiciousCsatRule)GetSuspiciousCsatDetection(10);

            var result = validDetection.IsValid();

            Assert.True(result);
        }

        [Fact]
        public void ShouldNotValidateSuspiciousCsatRuleWhenCsatNull()
        {
            var validDetection = (SuspiciousCsatRule)GetSuspiciousCsatDetection(null);
            validDetection.Message.Csat = null;

            var result = validDetection.IsValid();

            Assert.False(result);
        }

        [Fact]
        public async void ShouldMatchSuspiciousCsatRuleWhenCsatEqualToFive()
        {
            var validDetection = (SuspiciousCsatRule)GetSuspiciousCsatDetection(5);

            var result = await validDetection.IsMatchAsync();

            Assert.True(result);
        }

        [Fact]
        public async void ShouldNotMatchSuspiciousCsatRuleWhenCsatLessThanFive()
        {
            var validDetection = (SuspiciousCsatRule)GetSuspiciousCsatDetection(3);

            var result = await validDetection.IsMatchAsync();

            Assert.False(result);
        }
    }
}