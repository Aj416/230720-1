﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tigerspike.Solv.Infra.Data.Context;

namespace Tigerspike.Solv.Infra.Data.Migrations
{
    [DbContext(typeof(SolvDbContext))]
    [Migration("20190929154948_MELCGE-3370")]
    partial class MELCGE3370
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Advocate", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<decimal>("Csat")
                        .HasColumnType("decimal(3,2)");

                    b.Property<string>("PaymentAccountId")
                        .HasMaxLength(255);

                    b.Property<bool>("PaymentEmailVerified");

                    b.Property<bool>("PaymentMethodSetup");

                    b.Property<bool>("PracticeComplete");

                    b.Property<bool>("Practicing");

                    b.Property<bool>("ShowBrandNotification");

                    b.Property<bool>("VideoWatched");

                    b.HasKey("Id");

                    b.ToTable("Advocate");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.AdvocateApplication", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ApplicationStatus")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<bool>("CompletedEmailSent");

                    b.Property<string>("Country")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("DeletionHash")
                        .HasMaxLength(64);

                    b.Property<string>("Email")
                        .HasMaxLength(255);

                    b.Property<string>("FirstName")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("InvitationDate");

                    b.Property<bool>("InvitationEmailSent")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<string>("LastName")
                        .HasMaxLength(200);

                    b.Property<string>("Phone")
                        .HasMaxLength(30);

                    b.Property<bool>("ProfilingEmailSent");

                    b.Property<bool>("ResponseEmailSent")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<string>("Source")
                        .HasMaxLength(50);

                    b.Property<string>("State")
                        .HasMaxLength(2);

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("AdvocateApplication");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.AdvocateBrand", b =>
                {
                    b.Property<Guid>("AdvocateId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BrandId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Authorized");

                    b.Property<bool>("ContractAccepted");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<bool>("Enabled");

                    b.Property<bool>("Inducted");

                    b.Property<DateTime>("ModifiedDate");

                    b.HasKey("AdvocateId", "BrandId");

                    b.HasIndex("BrandId");

                    b.ToTable("AdvocateBrand");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.AdvocateInvoice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AdvocateId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<Guid>("InvoicingCycleId");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("ReferenceNumber")
                        .IsRequired();

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(15,2)");

                    b.HasKey("Id");

                    b.HasAlternateKey("ReferenceNumber");

                    b.HasIndex("AdvocateId");

                    b.HasIndex("InvoicingCycleId");

                    b.ToTable("AdvocateInvoice");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.AdvocateInvoiceLineItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AdvocateInvoiceId");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(15,2)");

                    b.Property<Guid>("BrandId");

                    b.Property<int>("TicketsCount");

                    b.HasKey("Id");

                    b.HasIndex("AdvocateInvoiceId");

                    b.HasIndex("BrandId");

                    b.ToTable("AdvocateInvoiceLineItem");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.ApiKey", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BrandId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<DateTime?>("RevokedDate");

                    b.Property<Guid?>("UserId");

                    b.HasKey("Id");

                    b.HasAlternateKey("Key");

                    b.HasIndex("BrandId");

                    b.HasIndex("UserId");

                    b.ToTable("ApiKey");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Brand", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BillingAgreementId");

                    b.Property<string>("BillingAgreementToken");

                    b.Property<string>("Contract")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Department");

                    b.Property<decimal>("FeePercentage")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(6,4)")
                        .HasDefaultValue(0.3m);

                    b.Property<string>("Induction")
                        .HasMaxLength(256);

                    b.Property<bool>("IsPractice");

                    b.Property<string>("Logo")
                        .HasMaxLength(256);

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("PaymentAccountId");

                    b.Property<string>("Thumbnail")
                        .HasMaxLength(256);

                    b.Property<decimal>("TicketPrice")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(15,2)")
                        .HasDefaultValue(3m);

                    b.HasKey("Id");

                    b.ToTable("Brand");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.BrandTicketPriceHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BrandId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<decimal>("TicketPrice")
                        .HasColumnType("decimal(15,2)");

                    b.Property<Guid?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("UserId");

                    b.ToTable("BrandTicketPriceHistory");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Client", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<Guid>("BrandId");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.ToTable("Client");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.ClientInvoice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BrandId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<decimal>("Fee")
                        .HasColumnType("decimal(15,2)");

                    b.Property<Guid>("InvoicingCycleId");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(15,2)");

                    b.Property<string>("ReferenceNumber")
                        .IsRequired();

                    b.Property<int>("TicketsCount");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(15,2)");

                    b.HasKey("Id");

                    b.HasAlternateKey("ReferenceNumber");

                    b.HasIndex("BrandId");

                    b.HasIndex("InvoicingCycleId");

                    b.ToTable("ClientInvoice");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Induction.AdvocateSectionItem", b =>
                {
                    b.Property<Guid>("AdvocateId");

                    b.Property<Guid>("SectionItemId");

                    b.HasKey("AdvocateId", "SectionItemId");

                    b.HasIndex("SectionItemId");

                    b.ToTable("InductionAdvocateSectionItem");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Induction.Section", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BrandId");

                    b.Property<bool>("Enabled");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.ToTable("InductionSection");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Induction.SectionItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Enabled");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<Guid>("SectionId");

                    b.Property<string>("Source")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("SectionId");

                    b.ToTable("InductionSectionItem");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.InvoicingCycle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("From");

                    b.Property<DateTime>("To");

                    b.HasKey("Id");

                    b.ToTable("InvoicingCycle");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Payment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AdvocateInvoiceLineItemId");

                    b.Property<int>("Amount");

                    b.Property<Guid>("ClientInvoiceId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("ReferenceNumber")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAlternateKey("ReferenceNumber");

                    b.HasIndex("AdvocateInvoiceLineItemId");

                    b.HasIndex("ClientInvoiceId");

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.Answer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ApplicationAnswerId");

                    b.Property<Guid?>("QuestionOptionId");

                    b.Property<string>("StaticAnswer")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("ApplicationAnswerId");

                    b.HasIndex("QuestionOptionId");

                    b.ToTable("ProfileAnswer");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.ApplicationAnswer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AdvocateApplicationId");

                    b.Property<Guid>("QuestionId");

                    b.HasKey("Id");

                    b.HasIndex("AdvocateApplicationId");

                    b.HasIndex("QuestionId");

                    b.ToTable("ProfileApplicationAnswer");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.Area", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Order");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.HasKey("Id");

                    b.ToTable("ProfileArea");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.ProfileBrand", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BrandName")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("ProfileBrands");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.Question", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AreaId");

                    b.Property<bool>("Enabled");

                    b.Property<bool>("Optional");

                    b.Property<int>("Order");

                    b.Property<Guid>("QuestionTypeId");

                    b.Property<string>("SubTitle")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.HasIndex("QuestionTypeId");

                    b.ToTable("ProfileQuestion");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.QuestionDependency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("QuestionId");

                    b.Property<Guid>("QuestionOptionId");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.HasIndex("QuestionOptionId");

                    b.ToTable("ProfileQuestionDependency");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.QuestionOption", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Enabled");

                    b.Property<bool>("Optional");

                    b.Property<int>("Order");

                    b.Property<Guid>("QuestionId");

                    b.Property<string>("SubText")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("ProfileQuestionOption");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.QuestionOptionDependency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("QuestionOptionDependencyTargetId");

                    b.Property<Guid>("QuestionOptionId");

                    b.HasKey("Id");

                    b.HasIndex("QuestionOptionId");

                    b.ToTable("ProfileQuestionOptionDependency");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.QuestionType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsAllRequired");

                    b.Property<bool>("IsMultiChoice");

                    b.Property<bool>("IsSlider");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.HasKey("Id");

                    b.ToTable("ProfileQuestionType");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.RejectionReason", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("RejectionReason");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Ticket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<sbyte>("AbandonedCount")
                        .HasColumnType("tinyint");

                    b.Property<Guid?>("AdvocateId");

                    b.Property<Guid?>("AdvocateInvoiceId");

                    b.Property<DateTime?>("AssignedDate");

                    b.Property<Guid>("BrandId");

                    b.Property<Guid?>("ClientInvoiceId");

                    b.Property<DateTime?>("ClosedDate");

                    b.Property<sbyte?>("Complexity")
                        .HasColumnType("tinyint");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<sbyte?>("Csat")
                        .HasColumnType("tinyint");

                    b.Property<Guid>("CustomerId");

                    b.Property<decimal>("Fee")
                        .HasColumnType("decimal(15,2)");

                    b.Property<DateTime?>("FirstAssignedDate");

                    b.Property<DateTime?>("FirstMessageDate");

                    b.Property<bool>("IsPractice");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(15,2)");

                    b.Property<string>("Question")
                        .IsRequired();

                    b.Property<string>("ReferenceId")
                        .HasMaxLength(50);

                    b.Property<sbyte>("RejectionCount")
                        .HasColumnType("tinyint");

                    b.Property<DateTime?>("ReservationExpiryDate");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("AdvocateId");

                    b.HasIndex("AdvocateInvoiceId");

                    b.HasIndex("BrandId");

                    b.HasIndex("ClientInvoiceId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("Question");

                    b.HasIndex("Status");

                    b.HasIndex("BrandId", "ReferenceId")
                        .IsUnique();

                    b.ToTable("Ticket");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.TicketRejectionHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<Guid>("TicketId");

                    b.HasKey("Id");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketRejectionHistory");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.TicketRejectionReason", b =>
                {
                    b.Property<Guid>("TicketRejectionHistoryId");

                    b.Property<int>("RejectionReasonId");

                    b.HasKey("TicketRejectionHistoryId", "RejectionReasonId");

                    b.HasIndex("RejectionReasonId");

                    b.ToTable("TicketRejectionReason");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.TicketStatusHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AdvocateId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("Status");

                    b.Property<Guid>("TicketId");

                    b.HasKey("Id");

                    b.HasIndex("AdvocateId");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketStatusHistory");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<bool>("Enabled");

                    b.Property<string>("FirstName")
                        .HasMaxLength(200);

                    b.Property<string>("LastName")
                        .HasMaxLength(200);

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Phone")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasAlternateKey("Email");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.WebHook", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BrandId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Secret")
                        .HasMaxLength(36);

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<Guid?>("UserId");

                    b.Property<int>("WebHookEvent");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("UserId");

                    b.ToTable("WebHook");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Advocate", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.AdvocateBrand", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Advocate")
                        .WithMany("Brands")
                        .HasForeignKey("AdvocateId");

                    b.HasOne("Tigerspike.Solv.Domain.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("AdvocateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.Brand", "Brand")
                        .WithMany("Advocates")
                        .HasForeignKey("BrandId");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.AdvocateInvoice", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Advocate", "Advocate")
                        .WithMany()
                        .HasForeignKey("AdvocateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.InvoicingCycle", "InvoicingCycle")
                        .WithMany()
                        .HasForeignKey("InvoicingCycleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.AdvocateInvoiceLineItem", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.AdvocateInvoice")
                        .WithMany("LineItems")
                        .HasForeignKey("AdvocateInvoiceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.Brand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.ApiKey", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.BrandTicketPriceHistory", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Client", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Brand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.ClientInvoice", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Brand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.InvoicingCycle", "InvoicingCycle")
                        .WithMany()
                        .HasForeignKey("InvoicingCycleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Induction.AdvocateSectionItem", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Advocate", "Advocate")
                        .WithMany("AdvocateSectionItems")
                        .HasForeignKey("AdvocateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.Induction.SectionItem", "SectionItem")
                        .WithMany("AdvocateSectionItems")
                        .HasForeignKey("SectionItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Induction.Section", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Brand", "Brand")
                        .WithMany("InductionSections")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Induction.SectionItem", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Induction.Section", "Section")
                        .WithMany("SectionItems")
                        .HasForeignKey("SectionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Payment", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.AdvocateInvoiceLineItem")
                        .WithMany()
                        .HasForeignKey("AdvocateInvoiceLineItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.ClientInvoice")
                        .WithMany()
                        .HasForeignKey("ClientInvoiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.Answer", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Profile.ApplicationAnswer", "ApplicationAnswer")
                        .WithMany("Answers")
                        .HasForeignKey("ApplicationAnswerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.Profile.QuestionOption", "QuestionOption")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionOptionId");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.ApplicationAnswer", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.AdvocateApplication", "AdvocateApplication")
                        .WithMany("ApplicationAnswers")
                        .HasForeignKey("AdvocateApplicationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.Profile.Question", "Question")
                        .WithMany("ApplicationAnswers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.Question", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Profile.Area", "Area")
                        .WithMany("Questions")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.Profile.QuestionType", "QuestionType")
                        .WithMany("Questions")
                        .HasForeignKey("QuestionTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.QuestionDependency", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Profile.Question", "Question")
                        .WithMany("QuestionDependencies")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.Profile.QuestionOption", "QuestionOption")
                        .WithMany("QuestionDependencies")
                        .HasForeignKey("QuestionOptionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.QuestionOption", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Profile.Question", "Question")
                        .WithMany("QuestionOptions")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Profile.QuestionOptionDependency", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Profile.QuestionOption", "QuestionOption")
                        .WithMany("QuestionOptionDependencies")
                        .HasForeignKey("QuestionOptionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Ticket", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Advocate", "Advocate")
                        .WithMany()
                        .HasForeignKey("AdvocateId");

                    b.HasOne("Tigerspike.Solv.Domain.Models.AdvocateInvoice")
                        .WithMany("Tickets")
                        .HasForeignKey("AdvocateInvoiceId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Tigerspike.Solv.Domain.Models.Brand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.ClientInvoice")
                        .WithMany("Tickets")
                        .HasForeignKey("ClientInvoiceId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Tigerspike.Solv.Domain.Models.User", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.TicketRejectionHistory", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Ticket")
                        .WithMany("RejectionHistory")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.TicketRejectionReason", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.RejectionReason")
                        .WithMany()
                        .HasForeignKey("RejectionReasonId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.TicketRejectionHistory")
                        .WithMany("Reasons")
                        .HasForeignKey("TicketRejectionHistoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.TicketStatusHistory", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Advocate")
                        .WithMany()
                        .HasForeignKey("AdvocateId");

                    b.HasOne("Tigerspike.Solv.Domain.Models.Ticket")
                        .WithMany("StatusHistory")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.WebHook", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tigerspike.Solv.Domain.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
