using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Commands.Validations;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SubmitProfileAnswersCommand : Command<bool>
	{
		public Guid AdvocateApplicationId { get; set; }

		public List<ApplicationAnswer> ApplicationAnswers { get; set; }

		public SubmitProfileAnswersCommand(Guid advocateApplicationId, List<ApplicationAnswer> applicationAnswers)
		{
			AdvocateApplicationId = advocateApplicationId;
			ApplicationAnswers = applicationAnswers;
		}

		public override bool IsValid()
		{
			ValidationResult = new SubmitProfileAnswersCommandValidator().Validate(this);
			return ValidationResult.IsValid;
		}
	}
}