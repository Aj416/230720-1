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
    public class RepeatCxEmailRuleTests
    {
        private readonly AutoMoqer Mocker = new AutoMoqer();

        private IRule GetRepeatCxEmailDetection(List<EmailInfo> emailInfos)
        {
            var serialInfoFForLastWeek = new List<ISerialNumber>();
            var serialInfoFForLastThreeDays = new List<ISerialNumber>();

            DetectFraudCommand message = Builder<DetectFraudCommand>.CreateNew()
                .WithFactory(() => new DetectFraudCommand(serialInfoFForLastWeek, serialInfoFForLastThreeDays, emailInfos.ToList<ICustomer>(), 0, 1, DateTime.Now, Guid.NewGuid(),
                    Guid.NewGuid(), string.Empty, Guid.NewGuid(), string.Empty, string.Empty, new Dictionary<string, string>(), string.Empty, string.Empty, string.Empty, "Sample Question"))
                .Build();

            var rule = Builder<RepeatCxEmailRule>
                .CreateNew()
                .WithFactory(() => new RepeatCxEmailRule(message, Guid.NewGuid()))
                .Build();

            return rule;
        }

        [Fact]
        public void ShouldValidateRepeatCxEmailRuleWhenEmailInfoCountGreaterThanZero()
        {
            var customerInfos = new List<EmailInfo>(){
                Builder<EmailInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.Email)
                .Build(),
                Builder<EmailInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.Email)
                .Build(),
            };

            var validDetection = (RepeatCxEmailRule)GetRepeatCxEmailDetection(customerInfos);

            var result = validDetection.IsValid();

            Assert.True(result);
        }

        [Fact]
        public void ShouldNotValidateRepeatCxEmailRuleWhenEmailInfoCountIsZero()
        {
            var customerInfos = new List<EmailInfo>();
            var validDetection = (RepeatCxEmailRule)GetRepeatCxEmailDetection(customerInfos);

            var result = validDetection.IsValid();

            Assert.False(result);
        }

        [Fact]
        public async void ShouldMatchRepeatCxEmailRuleWhenEmailInfoGreaterThanTwo()
        {
            var customerInfos = new List<EmailInfo>(){
                Builder<EmailInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.Email)
                .Build(),
                Builder<EmailInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.Email)
                .Build(),
                Builder<EmailInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.Email)
                .Build()
            };

            var validDetection = (RepeatCxEmailRule)GetRepeatCxEmailDetection(customerInfos);

            var result = await validDetection.IsMatchAsync();

            Assert.True(result);
        }

        [Fact]
        public async void ShouldNotMatchRepeatCxEmailRuleWhenEmailInfoLessThanTwo()
        {
            var customerInfos = new List<EmailInfo>();
            var validDetection = (RepeatCxEmailRule)GetRepeatCxEmailDetection(customerInfos);

            var result = await validDetection.IsMatchAsync();

            Assert.False(result);
        }
    }
}