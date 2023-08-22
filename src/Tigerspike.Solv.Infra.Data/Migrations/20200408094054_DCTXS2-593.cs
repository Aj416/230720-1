using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    public partial class DCTXS2593 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Phrase",
                table: "WhitelistPhrase",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "WhitelistPhrase",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "WebHook",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "WebHook",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "WebHook",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "User",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "TicketId",
                table: "TicketStatusHistory",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvocateId",
                table: "TicketStatusHistory",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TicketStatusHistory",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "TicketRejectionHistoryId",
                table: "TicketRejectionReason",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "TicketId",
                table: "TicketRejectionHistory",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TicketRejectionHistory",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "TicketMetadataItem",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "TicketId",
                table: "TicketMetadataItem",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "TicketEscalationConfig",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TicketEscalationConfig",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "Ticket",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientInvoiceId",
                table: "Ticket",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "Ticket",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvocateInvoiceId",
                table: "Ticket",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvocateId",
                table: "Ticket",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Ticket",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                table: "QuizQuestionOption",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "QuizQuestionOption",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuizId",
                table: "QuizQuestion",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "QuizQuestion",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Quiz",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ProfileQuestionType",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionOptionId",
                table: "ProfileQuestionOptionDependency",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionOptionDependencyTargetId",
                table: "ProfileQuestionOptionDependency",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ProfileQuestionOptionDependency",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                table: "ProfileQuestionOption",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ProfileQuestionOption",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionOptionId",
                table: "ProfileQuestionDependency",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                table: "ProfileQuestionDependency",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ProfileQuestionDependency",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionTypeId",
                table: "ProfileQuestion",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaId",
                table: "ProfileQuestion",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ProfileQuestion",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ProfileBrands",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ProfileArea",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                table: "ProfileApplicationAnswer",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvocateApplicationId",
                table: "ProfileApplicationAnswer",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ProfileApplicationAnswer",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionOptionId",
                table: "ProfileAnswer",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicationAnswerId",
                table: "ProfileAnswer",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ProfileAnswer",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientInvoiceId",
                table: "Payment",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvocateInvoiceLineItemId",
                table: "Payment",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Payment",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "InvoicingCycle",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "SectionId",
                table: "InductionSectionItem",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "InductionSectionItem",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "InductionSection",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "InductionSection",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "SectionItemId",
                table: "InductionAdvocateSectionItem",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvocateId",
                table: "InductionAdvocateSectionItem",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                table: "ClientInvoice",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "PlatformBillingDetailsId",
                table: "ClientInvoice",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "InvoicingCycleId",
                table: "ClientInvoice",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "ClientInvoice",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandBillingDetailsId",
                table: "ClientInvoice",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClientInvoice",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "Client",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Client",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "BrandTicketPriceHistory",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "BrandTicketPriceHistory",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BrandTicketPriceHistory",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuizId",
                table: "Brand",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BillingDetailsId",
                table: "Brand",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Brand",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BillingDetails",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ApiKey",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "ApiKey",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ApiKey",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "AdvocateInvoiceLineItem",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvocateInvoiceId",
                table: "AdvocateInvoiceLineItem",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AdvocateInvoiceLineItem",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                table: "AdvocateInvoice",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "PlatformBillingDetailsId",
                table: "AdvocateInvoice",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "InvoicingCycleId",
                table: "AdvocateInvoice",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvocateId",
                table: "AdvocateInvoice",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AdvocateInvoice",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "AdvocateBrand",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvocateId",
                table: "AdvocateBrand",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "AdvocateApplicationBrand",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvocateApplicationId",
                table: "AdvocateApplicationBrand",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AdvocateApplication",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Advocate",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Phrase",
                table: "WhitelistPhrase",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandId",
                table: "WhitelistPhrase",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "UserId",
                table: "WebHook",
                type: "varbinary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandId",
                table: "WebHook",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "WebHook",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "User",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "TicketId",
                table: "TicketStatusHistory",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "AdvocateId",
                table: "TicketStatusHistory",
                type: "varbinary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "TicketStatusHistory",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "TicketRejectionHistoryId",
                table: "TicketRejectionReason",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "TicketId",
                table: "TicketRejectionHistory",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "TicketRejectionHistory",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "TicketMetadataItem",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<byte[]>(
                name: "TicketId",
                table: "TicketMetadataItem",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandId",
                table: "TicketEscalationConfig",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "TicketEscalationConfig",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "CustomerId",
                table: "Ticket",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "ClientInvoiceId",
                table: "Ticket",
                type: "varbinary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandId",
                table: "Ticket",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "AdvocateInvoiceId",
                table: "Ticket",
                type: "varbinary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "AdvocateId",
                table: "Ticket",
                type: "varbinary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "Ticket",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "QuestionId",
                table: "QuizQuestionOption",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "QuizQuestionOption",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "QuizId",
                table: "QuizQuestion",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "QuizQuestion",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "Quiz",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "ProfileQuestionType",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "QuestionOptionId",
                table: "ProfileQuestionOptionDependency",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "QuestionOptionDependencyTargetId",
                table: "ProfileQuestionOptionDependency",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "ProfileQuestionOptionDependency",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "QuestionId",
                table: "ProfileQuestionOption",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "ProfileQuestionOption",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "QuestionOptionId",
                table: "ProfileQuestionDependency",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "QuestionId",
                table: "ProfileQuestionDependency",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "ProfileQuestionDependency",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "QuestionTypeId",
                table: "ProfileQuestion",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "AreaId",
                table: "ProfileQuestion",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "ProfileQuestion",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "ProfileBrands",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "ProfileArea",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "QuestionId",
                table: "ProfileApplicationAnswer",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "AdvocateApplicationId",
                table: "ProfileApplicationAnswer",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "ProfileApplicationAnswer",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "QuestionOptionId",
                table: "ProfileAnswer",
                type: "varbinary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "ApplicationAnswerId",
                table: "ProfileAnswer",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "ProfileAnswer",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "ClientInvoiceId",
                table: "Payment",
                type: "varbinary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "AdvocateInvoiceLineItemId",
                table: "Payment",
                type: "varbinary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "Payment",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "InvoicingCycle",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "SectionId",
                table: "InductionSectionItem",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "InductionSectionItem",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandId",
                table: "InductionSection",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "InductionSection",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "SectionItemId",
                table: "InductionAdvocateSectionItem",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "AdvocateId",
                table: "InductionAdvocateSectionItem",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                table: "ClientInvoice",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<byte[]>(
                name: "PlatformBillingDetailsId",
                table: "ClientInvoice",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "InvoicingCycleId",
                table: "ClientInvoice",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandId",
                table: "ClientInvoice",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandBillingDetailsId",
                table: "ClientInvoice",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "ClientInvoice",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandId",
                table: "Client",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "Client",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "UserId",
                table: "BrandTicketPriceHistory",
                type: "varbinary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandId",
                table: "BrandTicketPriceHistory",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "BrandTicketPriceHistory",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "QuizId",
                table: "Brand",
                type: "varbinary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "BillingDetailsId",
                table: "Brand",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "Brand",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "BillingDetails",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "UserId",
                table: "ApiKey",
                type: "varbinary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandId",
                table: "ApiKey",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "ApiKey",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandId",
                table: "AdvocateInvoiceLineItem",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "AdvocateInvoiceId",
                table: "AdvocateInvoiceLineItem",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "AdvocateInvoiceLineItem",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                table: "AdvocateInvoice",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<byte[]>(
                name: "PlatformBillingDetailsId",
                table: "AdvocateInvoice",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "InvoicingCycleId",
                table: "AdvocateInvoice",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "AdvocateId",
                table: "AdvocateInvoice",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "AdvocateInvoice",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandId",
                table: "AdvocateBrand",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "AdvocateId",
                table: "AdvocateBrand",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "BrandId",
                table: "AdvocateApplicationBrand",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "AdvocateApplicationId",
                table: "AdvocateApplicationBrand",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "AdvocateApplication",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "Advocate",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(Guid));
        }
    }
}
