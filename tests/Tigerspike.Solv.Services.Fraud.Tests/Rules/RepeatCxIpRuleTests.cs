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
    public class RepeatCxIpRuleTests
    {
        private readonly AutoMoqer Mocker = new AutoMoqer();

        private IRule GetRepeatCxIpDetection(List<IpInfo> ipInfos)
        {
            var serialInfoFForLastWeek = new List<ISerialNumber>();
            var serialInfoFForLastThreeDays = new List<ISerialNumber>();

            DetectFraudCommand message = Builder<DetectFraudCommand>.CreateNew()
                .WithFactory(() => new DetectFraudCommand(serialInfoFForLastWeek, serialInfoFForLastThreeDays, ipInfos.ToList<ICustomer>(), 0, 1, DateTime.Now, Guid.NewGuid(),
                    Guid.NewGuid(), string.Empty, Guid.NewGuid(), string.Empty, string.Empty, new Dictionary<string, string>(), string.Empty, string.Empty, string.Empty, "Sample Question"))
                .Build();

            var rule = Builder<RepeatCxIpRule>
                .CreateNew()
                .WithFactory(() => new RepeatCxIpRule(message, Guid.NewGuid()))
                .Build();

            return rule;
        }

        [Fact]
        public void ShouldValidateRepeatCxIpRuleWhenIpInfoCountGreaterThanZero()
        {
            var customerInfos = new List<IpInfo>(){
                Builder<IpInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.Ip)
                .Build(),
                Builder<IpInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.Ip)
                .Build(),
            };

            var validDetection = (RepeatCxIpRule)GetRepeatCxIpDetection(customerInfos);

            var result = validDetection.IsValid();

            Assert.True(result);
        }

        [Fact]
        public void ShouldNotValidateRepeatCxIpRuleWhenIpInfoCountIsZero()
        {
            var customerInfos = new List<IpInfo>();
            var validDetection = (RepeatCxIpRule)GetRepeatCxIpDetection(customerInfos);

            var result = validDetection.IsValid();

            Assert.False(result);
        }

        [Fact]
        public async void ShouldMatchRepeatCxIpRuleWhenIpInfoGreaterThanTwo()
        {
            var customerInfos = new List<IpInfo>(){
                Builder<IpInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.Ip)
                .Build(),
                Builder<IpInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.Ip)
                .Build(),
                Builder<IpInfo>.CreateNew()
                .With(x => x.TicketId,new Guid())
                .With(x => x.Key,CustomerDetail.Ip)
                .Build()
            };

            var validDetection = (RepeatCxIpRule)GetRepeatCxIpDetection(customerInfos);

            var result = await validDetection.IsMatchAsync();

            Assert.True(result);
        }

        [Fact]
        public async void ShouldNotMatchRepeatCxIpRuleWhenIpInfoLessThanTwo()
        {
            var customerInfos = new List<IpInfo>();
            var validDetection = (RepeatCxIpRule)GetRepeatCxIpDetection(customerInfos);

            var result = await validDetection.IsMatchAsync();

            Assert.False(result);
        }
    }
}