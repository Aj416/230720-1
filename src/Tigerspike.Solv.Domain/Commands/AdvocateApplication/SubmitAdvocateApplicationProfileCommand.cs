using System;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SubmitAdvocateApplicationProfileCommand : Command<bool>
	{
		/// <summary>
		/// Gets or sets profile answers.
		/// </summary>
		public ApplicationAnswer ApplicationAnswer { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="advocateApplicationId">AdvocateApplicationId identifier.</param>
		/// <param name="applicationAnswer">Application Answer.</param>
		public SubmitAdvocateApplicationProfileCommand(ApplicationAnswer applicationAnswer) =>
			ApplicationAnswer = applicationAnswer;

		public override bool IsValid() => ApplicationAnswer != null;
	}
}
