using System;
using AutoMoqCore;
using FizzWare.NBuilder;
using Tigerspike.Solv.Application.Commands.Fraud;
using Tigerspike.Solv.Services.Fraud.Rules;
using Xunit;

namespace Tigerspike.Solv.Services.Fraud.Tests.Rules
{
    public class ClosedByCustomerRuleTests
    {
        private readonly AutoMoqer Mocker = new AutoMoqer();

        private IRule GetClosedByCustomerDetection(bool isValid)
        {
            DetectFraudCommand message = Builder<DetectFraudCommand>.CreateNew()
                        .WithFactory(() => new DetectFraudCommand(null, null, null, new Guid(), null, isValid,
                            1, 1, string.Empty, new Guid(), string.Empty, new Guid()))
                        .Build();

            var rule = Builder<ClosedByCustomerRule>
                .CreateNew()
                .WithFactory(() => new ClosedByCustomerRule(message, new Guid()))
                .Build();

            return rule;
        }

        [Fact]
        public void ShouldValidateClosedByCustomerRuleWhenClosedByCustomer()
        {
            var validDetection = (ClosedByCustomerRule)GetClosedByCustomerDetection(true);

            var result = validDetection.IsValid();

            Assert.True(result);
        }

        [Fact]
        public void ShouldNotValidateClosedByCustomerRuleWhenNotClosedByCustomer()
        {
            var validDetection = (ClosedByCustomerRule)GetClosedByCustomerDetection(true);
            validDetection.Message.ClosedByCustomer = null;

            var result = validDetection.IsValid();

            Assert.False(result);
        }

        [Fact]
        public async void ShouldMatchClosedByCustomerRuleWhenClosedByCustomer()
        {
            var validDetection = (ClosedByCustomerRule)GetClosedByCustomerDetection(true);

            var result = await validDetection.IsMatchAsync();

            Assert.True(result);
        }

        [Fact]
        public async void ShouldNotMatchClosedByCustomerRuleWhenNotClosedByCustomer()
        {
            var validDetection = (ClosedByCustomerRule)GetClosedByCustomerDetection(false);

            var result = await validDetection.IsMatchAsync();

            Assert.False(result);
        }
    }
}