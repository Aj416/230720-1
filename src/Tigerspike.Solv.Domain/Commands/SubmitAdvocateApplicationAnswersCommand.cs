using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Commands.Validations;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SubmitAdvocateApplicationAnswersCommand : Command<bool>
	{
		public SubmitAdvocateApplicationAnswersCommand(Guid advocateApplicationId, List<ApplicationAnswer> applicationAnswers)
		{
			AdvocateApplicationId = advocateApplicationId;
			ApplicationAnswers = applicationAnswers;
		}

		public Guid AdvocateApplicationId { get; set; }

		public List<ApplicationAnswer> ApplicationAnswers { get; set; }

		public override bool IsValid()
		{
			ValidationResult = new SubmitAdvocateApplicationAnswersCommandValidator().Validate(this);
			return ValidationResult.IsValid;
		}
	}
}