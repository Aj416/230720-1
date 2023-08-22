using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Domain.Commands.Brand
{
	public class CreateQuizCommand : Command
	{
		public Guid BrandId { get; set; }
		public string Title { get; set; }
		public string FailureMessage { get; set; }
		public string SuccessMessage { get; set; }
		public string Description { get; set; }
		public int AllowedMistakes { get; set; }
		public List<(string name, bool isMultiChoice, List<(string text, bool correct)> options)> Questions { get; set; }

		public CreateQuizCommand(Guid brandId, string title, string description, string failureMessage, string successMessage, int allowedMistakes, List<(string name, bool isMultiChoice, List<(string text, bool correct)> options)> questions)
		{
			BrandId = brandId;
			Title = title;
			FailureMessage = failureMessage;
			SuccessMessage = successMessage;
			Description = description;
			AllowedMistakes = allowedMistakes;
			Questions = questions;
		}

		public override bool IsValid() => BrandId != Guid.Empty && Title.IsNotEmpty();
	}
}