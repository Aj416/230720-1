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
	public class IpMatchRuleTests
	{
		private readonly AutoMoqer Mocker = new AutoMoqer();

		private IRule GetIpMatchDetection(string customerIp, string solverIp)
		{
			var serials = new List<SerialNumber>(){
				Builder<SerialNumber>.CreateNew()
				.Build()
			};

			DetectFraudCommand message = Builder<DetectFraudCommand>.CreateNew()
						.WithFactory(() => new DetectFraudCommand(serials.ToList<ISerialNumber>(), 2, 1, new Guid(), string.Empty, new Guid(),
							string.Empty, new Guid(), customerIp, solverIp))
						.Build();

			var rule = Builder<IpMatchRule>
				.CreateNew()
				.WithFactory(() => new IpMatchRule(message, new Guid()))
				.Build();

			return rule;
		}

		[Fact]
		public void ShouldValidateIpMatchRuleWhenBothIpNotNullOrEmpty()
		{
			var validDetection = (IpMatchRule)GetIpMatchDetection("127.0.0.1", "127.0.0.1");

			var result = validDetection.IsValid();

			Assert.True(result);
		}

		[Fact]
		public void ShouldNotValidateIpMatchRuleWhenOneOrBothIpNullOrEmpty()
		{
			var validDetection = (IpMatchRule)GetIpMatchDetection(string.Empty, "127.0.0.1");

			var result = validDetection.IsValid();

			Assert.False(result);
		}

		[Fact]
		public async void ShouldMatchIpMatchRuleWhenBothIpEqual()
		{
			var validDetection = (IpMatchRule)GetIpMatchDetection("127.0.0.1", "127.0.0.1");

			var result = await validDetection.IsMatchAsync();

			Assert.True(result);
		}

		[Fact]
		public async void ShouldNotMatchIpMatchRuleWhenBothIpNotEqual()
		{
			var validDetection = (IpMatchRule)GetIpMatchDetection("127.0.0.1", "127.0.0.2");

			var result = await validDetection.IsMatchAsync();

			Assert.False(result);
		}
	}
}