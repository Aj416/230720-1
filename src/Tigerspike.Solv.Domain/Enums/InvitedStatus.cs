using System.ComponentModel.DataAnnotations;

namespace Tigerspike.Solv.Domain.Enums
{
	public enum InvitedStatus
	{
		[Display(Name = " ")]
		None = 0,
		[Display(Name = "Contract not started")]
		ContractNotStarted = 1,
		[Display(Name = "Induction not started")]
		InductionNotStarted = 2,
		[Display(Name = "Induction started")]
		InductionStarted = 3,
		[Display(Name = "Quiz started")]
		QuizStarted = 4,
		[Display(Name = "Quiz failed")]
		QuizFailed = 5


	}
}