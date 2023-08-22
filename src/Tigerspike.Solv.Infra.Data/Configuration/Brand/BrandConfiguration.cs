using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Configuration.Common;

namespace Tigerspike.Solv.Infra.Data.Configuration
{
	public class BrandConfiguration : IEntityTypeConfiguration<Brand>
	{
		public void Configure(EntityTypeBuilder<Brand> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();

			builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
			builder.Property(e => e.Code).HasMaxLength(50);
			builder.Property(e => e.ShortCode).HasMaxLength(20);
			builder.Property(e => e.ContractUrl).HasMaxLength(256);
			builder.Property(e => e.ContractInternalUrl).HasMaxLength(256);
			builder.Property(e => e.CreatedDate).IsRequired();
			builder.Property(e => e.ModifiedDate).IsRequired();
			builder.Property(e => e.Logo).HasMaxLength(256);
			builder.Property(e => e.Thumbnail).HasMaxLength(256);
			builder.Property(e => e.IsPractice).IsRequired();
			builder.Property(e => e.VatRate).PercentageColumnType();
			builder.Property(e => e.FeePercentage).PercentageColumnType().IsRequired().HasDefaultValue(0.3);
			builder.Property(e => e.TicketPrice).PriceColumnType().IsRequired();
			builder.Property(e => e.PaymentAccountId);
			builder.Property(e => e.BillingAgreementId);
			builder.Property(e => e.BillingAgreementToken);
			builder.Property(e => e.CreateTicketHeader).LongTextColumnType();
			builder.Property(e => e.CreateTicketSubheader).LongTextColumnType();
			builder.Property(e => e.CreateTicketInstructions).LongTextColumnType();
			builder.Property(e => e.InductionInstructions).LongTextColumnType();
			builder.Property(e => e.InductionDoneMessage).LongTextColumnType();
			builder.Property(e => e.UnauthorizedMessage).LongTextColumnType();
			builder.Property(e => e.AgreementHeading).HasMaxLength(256);
			builder.Property(e => e.AgreementContent).LongTextColumnType();
			builder.Property(e => e.IsAgreementRequired);
			builder.Property(e => e.QuizId);
			builder.Property(e => e.ProbingFormId);
			builder.Property(e => e.InvoicingEnabled).IsRequired();
			builder.Property(e => e.InvoicingDashboardEnabled).IsRequired();
			builder.Property(e => e.TagsEnabled).IsRequired();
			builder.Property(e => e.NpsEnabled).IsRequired();
			builder.Property(e => e.SuperSolversEnabled).IsRequired();
			builder.Property(e => e.PushBackToClientEnabled).IsRequired();
			builder.Property(e => e.TicketsExportEnabled).IsRequired();
			builder.Property(e => e.WaitMinutesToClose).IsRequired().HasDefaultValue(4320);
			builder.Property(e => e.Color).HasMaxLength(7);
			builder.Property(e => e.SposEmail).HasMaxLength(100);
			builder.Property(e => e.TicketsImportEnabled).IsRequired();
			builder.Property(e => e.SubTagEnabled).IsRequired();
			builder.Property(e => e.MultiTagEnabled).IsRequired();
			builder.Property(e => e.CategoryEnabled).IsRequired();
			builder.Property(e => e.ValidTransferEnabled).IsRequired();
			builder.Property(e => e.SposDescription);
			builder.Property(e => e.CategoryDescription);
			builder.Property(e => e.ValidTransferDescription);
			builder.Property(e => e.DiagnosisDescription);
			builder.Property(e => e.AdditionalFeedBackEnabled).IsRequired();
			builder.Property(e => e.EndChatEnabled).IsRequired();
			builder.Property(e => e.DefaultCulture).HasMaxLength(10);
			builder.Property(e => e.EnableCustomerEndpoint).HasDefaultValue(false);
			builder.Property(e => e.SkipCustomerForm).HasDefaultValue(false);
			builder.Property(e => e.ProbingFormRedirectUrl);

			builder.HasOne<BillingDetails>().WithMany().HasForeignKey(x => x.BillingDetailsId).IsRequired();
			builder.HasMany<Ticket>().WithOne().HasForeignKey(e => e.BrandId);
			builder.HasMany(x => x.AbandonReasons).WithOne().HasForeignKey(x => x.BrandId);
			builder.HasMany(x => x.Tags).WithOne().HasForeignKey(x => x.BrandId);
			builder.HasOne<Quiz>().WithOne(x => x.Brand).HasForeignKey<Brand>(x => x.QuizId);
			builder.HasOne(x => x.ProbingForm).WithOne(x => x.Brand).HasForeignKey<Brand>(x => x.ProbingFormId);
			builder.HasMany(x => x.Categories).WithOne().HasForeignKey(x => x.BrandId);

			builder.HasOne(e => e.PaymentRoute).WithMany().HasForeignKey(f => f.PaymentRouteId);
		}
	}
}