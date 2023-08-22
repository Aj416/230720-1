using FizzWare.NBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tigerspike.Solv.Application.Commands.Fraud;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Fraud;
using Tigerspike.Solv.Messaging.Fraud;
using Tigerspike.Solv.Services.Fraud.Rules;
using Xunit;

namespace Tigerspike.Solv.Services.Fraud.Tests.Rules
{
    public class RepeatSerialRuleTests
    {
        private IRule RepeatSerialDetection(List<SerialNumber> serialNumbers)
        {
            var customerInfos = new List<ICustomer>();
            var serialInfoForLastWeek = new List<ISerialNumber>();

            DetectFraudCommand message = Builder<DetectFraudCommand>.CreateNew()
                .WithFactory(() => new DetectFraudCommand(serialInfoForLastWeek, serialNumbers.ToList<ISerialNumber>(), customerInfos, 0, 1, DateTime.Now, Guid.NewGuid(),
                    Guid.NewGuid(), string.Empty, Guid.NewGuid(), string.Empty, string.Empty, new Dictionary<string, string>(), string.Empty, string.Empty, string.Empty, "Sample Question"))
                .Build();

            var rule = Builder<RepeatSerialRule>
                .CreateNew()
                .WithFactory(() => new RepeatSerialRule(message, Guid.NewGuid()))
                .Build();

            return rule;
        }

        [Fact]
        public void ShouldValidateRepeatSerialRuleWhenSerialNumberCountGreaterThanZero()
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

            var validDetection = (RepeatSerialRule)RepeatSerialDetection(serialNumbers);

            var result = validDetection.IsValid();

            Assert.True(result);
        }

        [Fact]
        public void ShouldNotValidateRepeatSerialRuleWhenSerialNumberCountIsZero()
        {
            var serialNumbers = new List<SerialNumber>();
            var validDetection = (RepeatSerialRule)RepeatSerialDetection(serialNumbers);

            var result = validDetection.IsValid();

            Assert.False(result);
        }

        [Fact]
        public async void ShouldMatchRepeatSerialRuleWhenSerialNumberGreaterThanOne()
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

            var validDetection = (RepeatSerialRule)RepeatSerialDetection(serialNumbers);

            var result = await validDetection.IsMatchAsync();

            Assert.True(result);
        }

        [Fact]
        public async void ShouldNotMatchRepeatSerialRuleWhenSerialNumberLessThanOne()
        {
            var serialNumbers = new List<SerialNumber>();
            var validDetection = (RepeatSerialRule)RepeatSerialDetection(serialNumbers);

            var result = await validDetection.IsMatchAsync();

            Assert.False(result);
        }
    }
}
