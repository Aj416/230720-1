using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class ImportTicketCommandValidator : AbstractValidator<ImportTicketCommand>
	{
		public ImportTicketCommandValidator(
			IBrandRepository brandRepository,
			ITicketImportRepository ticketImportRepository,
			ITicketSourceRepository ticketSourceRepository,
			ITicketRepository ticketRepository,
			IAdvocateRepository advocateRepository
		)
		{

			RuleFor(x => x.Question).NotEmpty();
			RuleFor(x => x.CustomerFirstName).NotEmpty().MaximumLength(200);
			RuleFor(x => x.CustomerLastName).NotEmpty().MaximumLength(200);
			RuleFor(x => x.CustomerEmail).NotEmpty().MaximumLength(100).EmailAddress();

			RuleFor(x => x.ImportId)
				.NotEmpty()
				.MustAsync(async (importId, token) => await ticketImportRepository.ExistsAsync(x => x.Id == importId))
				.WithErrorCode(ImportIdNotExists)
				.WithMessage("Provided import process does not exist");

			RuleFor(x => x.BrandId)
				.NotEmpty()
				.MustAsync(async (brandId, token) => await brandRepository.ExistsAsync(b => b.Id == brandId))
				.WithErrorCode(BrandIdNotExists)
				.WithMessage("Provided brand does not exist");

			RuleFor(x => x.Tags)
				.CustomAsync(async (tags, ctx, token) =>
				{
					if (tags?.Length > 0)
					{
						var cmd = ctx.ParentContext.InstanceToValidate as ImportTicketCommand;
						var activeTags = (await brandRepository.GetTags(cmd.BrandId)).Select(x => x.Id).ToList();
						var incorrectTags = tags.Except(activeTags).ToList();
						foreach (var tag in incorrectTags)
						{
							ctx.AddFailure(new ValidationFailure(ctx.PropertyName, $"Incorrect tag '{tag}'", tag));
						}
					}
				});

			RuleFor(x => x.Source)
				.MustAsync(async (src, token) => await ticketSourceRepository.ExistsAsync(x => x.Name == src))
				.When(x => x.Source.IsNotEmpty())
				.WithErrorCode(SourceNotExists)
				.WithMessage("Provided ticket source does not exist");

			RuleFor(x => x.ReferenceId)
				.NotEmpty()
				.MaximumLength(50)
				.MustAsync(async (cmd, referenceId, token) => await ticketRepository.CountAsync(x => x.ReferenceId == referenceId && x.BrandId == cmd.BrandId) == 0)
				.WithErrorCode(ReferenceIdNotUnique)
				.WithMessage("Provided ReferenceId is not unique for specified brand");


			RuleFor(x => x.AdvocateEmail)
				.MaximumLength(100)
				.EmailAddress()
				.CustomAsync(async (email, ctx, token) =>
				{
					if (email.IsNotEmpty())
					{
						var cmd = ctx.ParentContext.InstanceToValidate as ImportTicketCommand;
						var advocate = await advocateRepository.GetFirstOrDefaultAsync(
							predicate: x => x.User.Email == email || x.User.AlternateEmail == email,
							include: x => x.Include(i => i.Brands)
						);

						if (advocate == null)
						{
							ctx.AddFailure(new ValidationFailure(ctx.PropertyName, "Provided Advocate email is not registered in the platform", email) { ErrorCode = AdvocateNotExists });
						}

						if (advocate != null && !advocate.Brands.Any(x => x.BrandId == cmd.BrandId && x.Authorized))
						{
							ctx.AddFailure(new ValidationFailure(ctx.PropertyName,
								"Provided Advocate email is not authorised for selected brand", email)
							{
								ErrorCode = AdvocateNotAuthorised
							});
						}
					}
				});

			RuleFor(x => x.AdvocateEmail).NotEmpty()
				.When(x => x.AssignedDate != null);

			RuleFor(x => x.CreatedDate).NotEmpty();

			RuleFor(x => x.AssignedDate).NotEmpty()
				.GreaterThanOrEqualTo(x => x.CreatedDate)
				.When(x => x.AdvocateEmail != null);

			RuleFor(x => x.SolvedDate)
				.GreaterThanOrEqualTo(x => x.AssignedDate)
				.When(x => x.SolvedDate != null);

			RuleFor(x => x.ClosedDate)
				.GreaterThanOrEqualTo(x => x.SolvedDate)
				.When(x => x.ClosedDate != null);

			RuleFor(x => x.Price)
				.GreaterThanOrEqualTo(0.00m)
				.When(x => x.Price != null);

			RuleFor(x => x.Csat)
				.InclusiveBetween(Domain.Models.Ticket.MIN_CSAT, Domain.Models.Ticket.MAX_CSAT)
				.When(x => x.Csat != null);

			RuleFor(x => x.Csat)
				.Must((cmd, csat) => cmd.ClosedDate != null)
				.When(x => x.Csat != null)
				.WithMessage($"{nameof(ImportTicketCommand.ClosedDate)} must be provided in order to set CSAT on a ticket");

			RuleFor(x => x.Complexity)
				.InclusiveBetween(Domain.Models.Ticket.MIN_COMPLEXITY, Domain.Models.Ticket.MAX_COMPLEXITY)
				.When(x => x.Complexity != null);

			RuleFor(x => x.Complexity)
				.Must((cmd, complexity) => cmd.ClosedDate != null)
				.When(x => x.Complexity != null)
				.WithMessage($"{nameof(ImportTicketCommand.Complexity)} must be provided in order to set Complexity on a ticket");

		}

		public const string ImportIdNotExists = nameof(ImportIdNotExists);
		public const string BrandIdNotExists = nameof(BrandIdNotExists);
		public const string SourceNotExists = nameof(SourceNotExists);
		public const string ReferenceIdNotUnique = nameof(ReferenceIdNotUnique);
		public const string AdvocateNotAuthorised = nameof(AdvocateNotAuthorised);
		public const string AdvocateNotExists = nameof(AdvocateNotExists);

	}
}