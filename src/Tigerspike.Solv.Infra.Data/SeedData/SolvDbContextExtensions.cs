using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.SeedData.Factories;

namespace Tigerspike.Solv.Infra.Data.SeedData
{
	public static class SolvDbContextExtensions
	{
		/// <summary>
		/// Ensures that seed data is in the database.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="solvBrandOptions"></param>
		/// <param name="demoBrandOptions"></param>
		public static async Task EnsureSeedData(this SolvDbContext context, SolvBrandOptions solvBrandOptions,
			DemoBrandOptions demoBrandOptions)
		{
			var baseDir = $"{AppDomain.CurrentDomain.BaseDirectory}SeedData/Scripts";

			if (context.AllMigrationsApplied())
			{
				// Use fluent api when possible and scripts when inserting is rather tricky via the
				// api

				if (!await context.Questions.AnyAsync())
				{
					// Profiling data script since it was rather difficult to do it with fluent api
					var profilingSql = await File.ReadAllTextAsync(Path.Combine(baseDir, "profiling.sql"));
					await context.Database.ExecuteSqlRawAsync(profilingSql);
				}

				await EnsureBrands(context, solvBrandOptions, demoBrandOptions);

				var rejectionReasonSet = context.Set<RejectionReason>();
				if (!await rejectionReasonSet.AnyAsync())
				{
					await rejectionReasonSet.AddRangeAsync(RejectionReasonFactory.GetList().Select(r => new RejectionReason { Name = r }));
				}

				if (await rejectionReasonSet.AnyAsync(x => x.Id == RejectionReason.ReservationExpiredReasonId) == false)
				{
					await rejectionReasonSet.AddAsync(new RejectionReason { Id = RejectionReason.ReservationExpiredReasonId, Name = RejectionReason.ReservationExpiredReasonName });
				}

				var inductionSections = context.InductionSections;
				if (!await inductionSections.AnyAsync())
				{
					await inductionSections.AddRangeAsync(InductionSectionFactory.GetList());
				}

				await EnsureApiKeys(context);
				await EnsureTicketSources(context);
				await EnsureAbandonReasons(context);
				await EnsureEscalationFlowConfig(context);
				await EnsureBrandNotificationConfig(context);

				if (context.ChangeTracker.HasChanges())
				{
					await context.SaveChangesAsync();
				}

				var usersSet = context.Set<User>();

				if (!await usersSet.AnyAsync())
				{
					// Seeding base admin/client accounts and few test advocates.
					var usersSql = File.ReadAllText(Path.Combine(baseDir, "users.sql"));
					context.Database.ExecuteSqlRaw(usersSql);
				}

				// Check if quartz tables are in the database
				var quartzSql = await File.ReadAllTextAsync(Path.Combine(baseDir, "quartz.sql"));
				await context.Database.ExecuteSqlRawAsync(quartzSql);
			}
		}

		private static async Task EnsureBrands(SolvDbContext context, SolvBrandOptions solvBrandOptions, DemoBrandOptions demoBrandOptions)
		{
			var brandSet = context.Set<Brand>();

			if (!await brandSet.AnyAsync(b => b.Id == Brand.PracticeBrandId))
			{
				AddBrand(context, Brand.CreatePracticeBrand(solvBrandOptions.Name, solvBrandOptions.LogoUrl, solvBrandOptions.ThumbnailUrl));
			}

			if (!await brandSet.AnyAsync(b => b.Id == Brand.DemoBrandId))
			{
				AddBrand(context, Brand.CreateDemoBrand(demoBrandOptions.Name, demoBrandOptions.LogoUrl, demoBrandOptions.ThumbnailUrl));
			}

			await EnsureBrandFormFieldTypes(context);

			if (context.ChangeTracker.HasChanges())
			{
				// save added brands, so they are available for other initialization methods
				await context.SaveChangesAsync();
			}
		}

		private static void AddBrand(SolvDbContext context, Brand brand)
		{
			var brandSet = context.Set<Brand>();
			var brandTicketPriceHistorySet = context.Set<BrandTicketPriceHistory>();
			brandSet.Add(brand);
			brandTicketPriceHistorySet.Add(new BrandTicketPriceHistory(brand.Id, brand.TicketPrice, null));
		}

		private static async Task EnsureBrandFormFieldTypes(SolvDbContext context)
		{
			var set = context.Set<BrandFormFieldType>();
			if (await set.CountAsync() == 0)
			{
				set.Add(new BrandFormFieldType { Id = 1, Name = "input"} );
				set.Add(new BrandFormFieldType { Id = 2, Name = "textarea" });
				set.Add(new BrandFormFieldType { Id = 3, Name = "select" });
			}
		}

		private static async Task EnsureApiKeys(SolvDbContext context)
		{
			var apiKeySet = context.Set<ApiKey>();
			var demoApiKey = await apiKeySet.FirstOrDefaultAsync(x => x.BrandId == Brand.DemoBrandId);
			if (demoApiKey == null)
			{
				apiKeySet.Add(new ApiKey(Brand.DemoBrandId, ApiKey.DemoApiKey, ApiKey.DemoApplicationId));
			}
		}

		private static async Task EnsureAbandonReasons(SolvDbContext context)
		{
			var set = context.Set<AbandonReason>();
			var brands = await context.Set<Brand>()
				.Include(x => x.AbandonReasons)
				.ToListAsync();

			foreach (var brand in brands)
			{
				if (brand.AbandonReasons.Count == 0)
				{
					brand.SetForcedEscalationReason(AbandonReason.ForcedEscalationReasonName);
					brand.SetBlockedAdvocateReason(AbandonReason.BlockedAdvocateReasonName);
					brand.SetAutoAbandonedReason(AbandonReason.AutoAbandonedReasonName);

					foreach (var reason in AbandonReasonFactory.GetList())
					{
						brand.AddAbandonReason(reason);
					}
				}
			}
		}

		private static async Task EnsureTicketSources(SolvDbContext context)
		{
			var set = context.Set<TicketSource>();
			if (await set.CountAsync() == 0)
			{
				set.Add(new TicketSource() { Name = "Webform" });
				set.Add(new TicketSource() { Name = "Chat" });
			}
		}

		private static async Task EnsureEscalationFlowConfig(SolvDbContext context)
		{
			var set = context.Set<TicketEscalationConfig>();
			if (await set.CountAsync() == 0)
			{
				set.Add(new TicketEscalationConfig()
				{
					BrandId = Brand.DemoBrandId,
					AbandonedCount = 1,
					RejectionCount = 2,
					CustomerMessage = "Your ticket is escalated! You can close this window now."
				});
			}
		}

		private static async Task EnsureBrandNotificationConfig(SolvDbContext context)
		{
			var set = context.Set<BrandNotificationConfig>();
			if (await set.CountAsync() == 0)
			{
				set.Add(new BrandNotificationConfig(Brand.DemoBrandId, true, BrandNotificationType.TicketSolved_Chat, 120,
					"{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}",
					"{{AdvocateFirstName}} has marked your support ticket as solved",
					"Please note, if we do not hear from you within the next <b>7 days</b>, your support ticket will be closed")
				);

				set.Add(new BrandNotificationConfig(Brand.DemoBrandId, true, BrandNotificationType.TicketSolved_Chat, 180,
					"{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}",
					"We noticed that your support ticket is still open at your end",
					"Please note, if we do not hear from you within the next <b>4 days</b>, your support ticket will be closed")
				);

				set.Add(new BrandNotificationConfig(Brand.DemoBrandId, true, BrandNotificationType.TicketSolved_Chat, 300,
					"{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}",
					"Your support ticket will be closed in the next 48 hours",
					"Please note, if we do not hear from you within the next <b>2 days</b>, your support ticket will be closed")
				);
			}
		}

		/// <summary>
		/// Checks if all migrations have already been executed.
		/// </summary>
		/// <param name="context">The database context.</param>
		/// <returns>True if migrations have been executed, false otherwise.</returns>
		private static bool AllMigrationsApplied(this SolvDbContext context)
		{
			var applied = context.GetService<IHistoryRepository>()
				.GetAppliedMigrations()
				.Select(m => m.MigrationId);

			var total = context.GetService<IMigrationsAssembly>()
				.Migrations
				.Select(m => m.Key);

			return !total.Except(applied).Any();
		}
	}
}