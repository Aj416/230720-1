using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoqCore;
using FizzWare.NBuilder;
using Moq;
using Tigerspike.Solv.Application.Commands.Fraud;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Fraud;
using Tigerspike.Solv.Messaging.Fraud;
using Tigerspike.Solv.Services.Fraud.Rules;
using Xunit;

namespace Tigerspike.Solv.Services.Fraud.Tests.Rules
{
    public class RepeatSerialDifferentCxRuleTests
    {
        private readonly AutoMoqer Mocker = new AutoMoqer();

        private IRule RepeatSerialDifferentCxDetection(List<SerialNumber> serialNumbers)
        {
            var customerInfos = new List<ICustomer>();
            var serialInfoFForLastThreeDays = new List<ISerialNumber>();

            DetectFraudCommand message = Builder<DetectFraudCommand>.CreateNew()
                .WithFactory(() => new DetectFraudCommand(serialNumbers.ToList<ISerialNumber>(), serialInfoFForLastThreeDays, customerInfos, 0, 1, DateTime.Now, Guid.NewGuid(),
                    Guid.NewGuid(), string.Empty, Guid.NewGuid(), string.Empty, string.Empty, new Dictionary<string, string>(), string.Empty, string.Empty, string.Empty, "Sample Question"))
                .Build();

            var rule = Builder<RepeatSerialDifferentCxRule>
                .CreateNew()
                .WithFactory(() => new RepeatSerialDifferentCxRule(message, Guid.NewGuid()))
                .Build();

            return rule;
        }

        [Fact]
        public void ShouldValidateRepeatSerialDifferentCxRuleWhenSerialNumberCountGreaterThanZero()
        {
            var serialNumbers = new List<SerialNumber>(){
                Builder<SerialNumber>.CreateNew()
                .With(x => x.CustomerId,new Guid())
                .With(x => x.TicketId,new Guid())
                .Build(),
                Builder<SerialNumber>.CreateNew()
                .With(x => x.CustomerId,new Guid())
                .With(x => x.TicketId,new Guid())
                .Build(),
            };

            var validDetection = (RepeatSerialDifferentCxRule)RepeatSerialDifferentCxDetection(serialNumbers);

            var result = validDetection.IsValid();

            Assert.True(result);
        }

        [Fact]
        public void ShouldNotValidateRepeatSerialDifferentCxRuleWhenSerialNumberCountIsZero()
        {
            var serialNumbers = new List<SerialNumber>();
            var validDetection = (RepeatSerialDifferentCxRule)RepeatSerialDifferentCxDetection(serialNumbers);

            var result = validDetection.IsValid();

            Assert.False(result);
        }

        [Fact]
        public async void ShouldMatchRepeatSerialDifferentCxRuleWhenSerialNumberGreaterThanOne()
        {
            var serialNumbers = new List<SerialNumber>(){
                Builder<SerialNumber>.CreateNew()
                .With(x => x.CustomerId,new Guid())
                .With(x => x.TicketId,new Guid())
                .Build(),
                Builder<SerialNumber>.CreateNew()
                .With(x => x.CustomerId,new Guid())
                .With(x => x.TicketId,new Guid())
                .Build()
            };

            var validDetection = (RepeatSerialDifferentCxRule)RepeatSerialDifferentCxDetection(serialNumbers);

            var result = await validDetection.IsMatchAsync();

            Assert.True(result);
        }

        [Fact]
        public async void ShouldNotMatchRepeatSerialDifferentCxRuleWhenSerialNumberLessThanOne()
        {
            var serialNumbers = new List<SerialNumber>();
            var validDetection = (RepeatSerialDifferentCxRule)RepeatSerialDifferentCxDetection(serialNumbers);

            var result = await validDetection.IsMatchAsync();

            Assert.False(result);
        }
    }
}