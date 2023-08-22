using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Tigerspike.Solv.Application.Commands.Fraud;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Fraud;
using Tigerspike.Solv.Messaging.Fraud;
using Tigerspike.Solv.Services.Fraud.Rules;
using Xunit;

namespace Tigerspike.Solv.Services.Fraud.Tests.Rules
{
    public class RepeatCxNameRuleTests
    {
        private IRule RepeatCxNameDetection(List<FullNameInfo> fullNameInfos)
        {
            var serialInfoFForLastWeek = new List<ISerialNumber>();
            var serialInfoFForLastThreeDays = new List<ISerialNumber>();

            DetectFraudCommand message = Builder<DetectFraudCommand>.CreateNew()
                .WithFactory(() => new DetectFraudCommand(serialInfoFForLastWeek, serialInfoFForLastThreeDays, fullNameInfos.ToList<ICustomer>(), 0, 1, DateTime.Now, Guid.NewGuid(),
                    Guid.NewGuid(), string.Empty, Guid.NewGuid(), string.Empty, string.Empty, new Dictionary<string, string>(), string.Empty, string.Empty, string.Empty, "Sample Question"))
                .Build();

            var rule = Builder<RepeatCxNameRule>
                .CreateNew()
                .WithFactory(() => new RepeatCxNameRule(message, Guid.NewGuid()))
                .Build();

            return rule;
        }

        [Fact]
        public void ShouldValidateRepeatCxNameRuleWhenFullNameInfoCountGreaterThanZero()
        {
            var customerInfos = new List<FullNameInfo>(){
                Builder<FullNameInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.FullName)
                .Build(),
                Builder<FullNameInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.FullName)
                .Build(),
            };

            var validDetection = (RepeatCxNameRule)RepeatCxNameDetection(customerInfos);

            var result = validDetection.IsValid();

            Assert.True(result);
        }

        [Fact]
        public void ShouldNotValidateRepeatCxNameRuleWhenFullNameInfoCountIsZero()
        {
            var customerInfos = new List<FullNameInfo>();
            var validDetection = (RepeatCxNameRule)RepeatCxNameDetection(customerInfos);

            var result = validDetection.IsValid();

            Assert.False(result);
        }

        [Fact]
        public async void ShouldMatchRepeatCxNameRuleWhenFullNameInfoGreaterThanTwo()
        {
            var customerInfos = new List<FullNameInfo>(){
                Builder<FullNameInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.FullName)
                .Build(),
                Builder<FullNameInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.FullName)
                .Build(),
                Builder<FullNameInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.FullName)
                .Build()
            };

            var validDetection = (RepeatCxNameRule)RepeatCxNameDetection(customerInfos);

            var result = await validDetection.IsMatchAsync();

            Assert.True(result);
        }

        [Fact]
        public async void ShouldNotMatchRepeatCxNameRuleWhenFullNameInfoLessThanTwo()
        {
            var customerInfos = new List<FullNameInfo>();
            var validDetection = (RepeatCxNameRule)RepeatCxNameDetection(customerInfos);

            var result = await validDetection.IsMatchAsync();

            Assert.False(result);
        }
    }
}