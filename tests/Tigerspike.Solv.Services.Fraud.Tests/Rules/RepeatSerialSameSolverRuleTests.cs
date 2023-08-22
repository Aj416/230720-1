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
	public class RepeatSerialSameSolverRuleTests
	{
		private readonly AutoMoqer Mocker = new AutoMoqer();

		private IRule RepeatSerialSameSolverDetection(List<SerialNumber> serialNumbers)
		{
			DetectFraudCommand message = Builder<DetectFraudCommand>.CreateNew()
				.WithFactory(() => new DetectFraudCommand(serialNumbers.ToList<ISerialNumber>(), 2, 1, new Guid(), string.Empty, new Guid(),
							string.Empty, new Guid(), string.Empty, string.Empty))
				.Build();

			var rule = Builder<RepeatSerialSameSolverRule>
				.CreateNew()
				.WithFactory(() => new RepeatSerialSameSolverRule(message, new Guid()))
				.Build();

			return rule;
		}

		[Fact]
		public void ShouldValidateRepeatSerialSameSolverRuleWhenSerialNumberCountGreaterThanZero()
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

			var validDetection = (RepeatSerialSameSolverRule)RepeatSerialSameSolverDetection(serialNumbers);

			var result = validDetection.IsValid();

			Assert.True(result);
		}

		[Fact]
		public void ShouldNotValidateRepeatSerialSameSolverRuleWhenSerialNumberCountIsZero()
		{
			var serialNumbers = new List<SerialNumber>();
			var validDetection = (RepeatSerialSameSolverRule)RepeatSerialSameSolverDetection(serialNumbers);

			var result = validDetection.IsValid();

			Assert.False(result);
		}

		[Fact]
		public async void ShouldMatchRepeatSerialSameSolverRuleWhenSerialNumberGreaterThanOne()
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

			var validDetection = (RepeatSerialSameSolverRule)RepeatSerialSameSolverDetection(serialNumbers);

			var result = await validDetection.IsMatchAsync();

			Assert.True(result);
		}

		[Fact]
		public async void ShouldNotMatchRepeatSerialSameSolverRuleWhenSerialNumberLessThanOne()
		{
			var serialNumbers = new List<SerialNumber>();
			var validDetection = (RepeatSerialSameSolverRule)RepeatSerialSameSolverDetection(serialNumbers);

			var result = await validDetection.IsMatchAsync();

			Assert.False(result);
		}
	}
}