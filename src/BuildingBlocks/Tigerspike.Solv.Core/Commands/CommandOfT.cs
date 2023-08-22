using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Tigerspike.Solv.Core.Commands
{
	public abstract class Command<T> : CommandBase, IRequest<T>
	{

	}

	/// <summary>
	/// This class should replace the one above
	/// We will gradually move from the commands that inherits the one above to inherit the one below
	/// </summary>
	/// <typeparam name="T">The command</typeparam>
	/// <typeparam name="TValidator">The validator of the command</typeparam>
	public abstract class Command<T, TValidator> : Command<T>
		where TValidator : IValidator, new()
	{
		public override bool IsValid()
		{
			var validator = new TValidator();
			var ctx = new ValidationContext<Command<T>>(this);
			ValidationResult = validator.Validate(ctx);
			return ValidationResult.IsValid;
		}
	}
}