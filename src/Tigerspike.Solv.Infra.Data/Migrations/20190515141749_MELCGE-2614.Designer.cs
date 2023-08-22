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
    [Migration("20190515141749_MELCGE-2614")]
    partial class MELCGE2614
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Advocate", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<decimal>("Csat")
                        .HasColumnType("decimal(3,2)");

                    b.Property<string>("PaymentAccountId")
                        .HasMaxLength(255);

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
                    b.Property<Guid>("AdvocateApplicationId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Country")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<string>("DeletionHash")
                        .HasMaxLength(64);

                    b.Property<string>("Email")
                        .HasMaxLength(255);

                    b.Property<string>("FullName")
                        .HasMaxLength(255);

                    b.Property<bool>("InvitationEmailSent")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<string>("State")
                        .HasMaxLength(2);

                    b.HasKey("AdvocateApplicationId");

                    b.HasIndex("AdvocateApplicationId");

                    b.ToTable("AdvocateApplication");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .HasMaxLength(100);

                    b.Property<bool>("EmailVerified");

                    b.Property<bool>("Enabled");

                    b.Property<string>("FirstName")
                        .HasMaxLength(200);

                    b.Property<string>("LastName")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Tigerspike.Solv.Domain.Models.Advocate", b =>
                {
                    b.HasOne("Tigerspike.Solv.Domain.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
