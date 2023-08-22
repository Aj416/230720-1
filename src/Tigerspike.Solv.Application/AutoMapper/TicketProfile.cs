using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Tigerspike.Solv.Application.Interfaces.Contracts;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Chat;
using Tigerspike.Solv.Application.Models.Fraud;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Models;
using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Application.AutoMapper
{
	public class TicketProfile : Profile
	{
		public TicketProfile()
		{

			CreateMap<Ticket, TicketModel>()
				.ForMember(dst => dst.Level, opt => opt.MapFrom(src => (int)src.Level))
				.ForMember(dst => dst.Metadata, opt => opt.MapFrom(src => new Dictionary<string, string>(src.Metadata.OrderBy(x => x.Order ?? int.MaxValue).Select(x => KeyValuePair.Create(x.Key, x.Value)))))
				.ForMember(dst => dst.ProbingAnswers, opt => opt.MapFrom(src => src.ProbingAnswers.Select(x => new ProbingAnswerModel(
					x.ProbingQuestion.Text,
					x.ProbingQuestion.Code,
					x.ProbingQuestionOption != null ? x.ProbingQuestionOption.Text : null,
					x.ProbingQuestionOption != null ? x.ProbingQuestionOption.Action : null
				))))
				.ForMember(dst => dst.AllAdvocates, opt => opt.MapFrom(x => x.StatusHistory.Where(y => y.AdvocateId != null).Select(y => y.Advocate).Distinct()))
				.ForMember(dst => dst.Tags, opt => opt.MapFrom(x => MapToTagModel(x.Tags.Where(t => t.Level == x.Level).Select(y => y.Tag).ToList())))
				.ForMember(dst => dst.ServerTimestamp, opt => opt.MapFrom<TimestampPropertyResolver>())
				.ForMember(dst => dst.Source, opt => opt.MapFrom(x => x.Source != null ? x.Source.Name : TicketSource.DefaultName))
				.ForMember(dst => dst.IsCustomerOnline, opt => opt.Ignore())
				.ForMember(dst => dst.MaxSolverResponseTime, opt => opt.MapFrom(src => src.SolverMaxResponseTimeInSeconds))
				.ForMember(dst => dst.TagStatus, opt => opt.MapFrom(src => src.TagStatus))
				.ForMember(dst => dst.IsTagged, opt => opt.MapFrom(x => x.TagStatus != null || x.IsTaggingComplete()))
				.ForMember(dst => dst.DisabledTags, opt => opt.MapFrom(x => new List<Guid>()))
				.ForMember(dst => dst.Category, opt => opt.MapFrom(x => x.TicketCategory.Category))
				.ForMember(dst => dst.Ready, opt => opt.MapFrom(x => x.Ready));

			CreateMap<Ticket, TicketPrintableModel>();

			CreateMap<Ticket, Models.Customer.CustomerTicketModel>()
			.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dst => dst.Question, opt => opt.MapFrom(src => src.Question))
			.ForMember(dst => dst.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
			.ForMember(dst => dst.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
			.ForMember(dst => dst.ChatUrl, opt => opt.Ignore());

			CreateMap<TicketAbandonHistory, TicketAbandonHistoryModel>()
				.ForMember(x => x.Reasons, opt => opt.MapFrom(src => src.Reasons.Select(x => x.AbandonReason.Name)));

			CreateMap<Ticket, TicketSearchModel>()
				.IncludeBase<Ticket, TicketModel>()
				.ForMember(dst => dst.Advocate, opt => opt.Ignore())
				.ForMember(dst => dst.ServerTimestamp, opt => opt.Ignore())
				.ForMember(dst => dst.Tags, opt => opt.Ignore())
				.ForMember(dst => dst.AllAdvocates, opt => opt.Ignore())
				.ForMember(dst => dst.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
				.ForMember(dst => dst.CurrentAdvocateFullName, opt => opt.MapFrom(x => x.Advocate != null ? x.Advocate.User.FullName : string.Empty))
				.ForMember(dst => dst.LastAbandonmentReasonsNames, opt => opt.Ignore())
				.ForMember(dst => dst.AdvocateFullName, opt => opt.Ignore())
				.ForMember(dst => dst.Category, opt => opt.Ignore())
				.ForMember(dst => dst.AdvocatesHistory,
					opt => opt.MapFrom(src => src.StatusHistory
						.Where(x => x.AdvocateId != null)
						.OrderBy(x => x.CreatedDate)
						.Select(x => x.Advocate.User.FullName)
					)
				);

			CreateMap<Ticket, ITicketExportContract>()
				.ForMember(x => x.Metadata, opt => opt.MapFrom(x => x.FlatMetadata))
				.ForMember(x => x.BrandName, opt => opt.MapFrom(x => x.Brand.Name))
				.ForMember(x => x.TicketSource, opt => opt.MapFrom(x => x.Source != null ? x.Source.Name : ""))
				.ForMember(x => x.L1Tags, opt => opt.MapFrom(x => FlattenTags(x.Tags.Where(t => t.Level == TicketLevel.Regular).ToList())))
				.ForMember(x => x.L2Tags, opt => opt.MapFrom(x => FlattenTags(x.Tags.Where(t => t.Level == TicketLevel.SuperSolver).ToList())))
				.ForMember(x => x.L1SubTags, opt => opt.MapFrom(x => FlattenSubTags(x.Tags.Where(t => t.Level == TicketLevel.Regular).ToList())))
				.ForMember(x => x.L2SubTags, opt => opt.MapFrom(x => FlattenSubTags(x.Tags.Where(t => t.Level == TicketLevel.SuperSolver).ToList())))
				.ForMember(x => x.ChaserEmails, opt => opt.MapFrom(x => x.ChaserEmails != null ? x.ChaserEmails : 0))
				.ForMember(x => x.Complexity, opt => opt.MapFrom(x => x.Complexity > 0 ? x.Complexity : null))
				.ForMember(x => x.CustomerQuery, opt => opt.MapFrom(x => x.Question))
				.ForMember(x => x.DateEscalatedToL2, opt => opt.MapFrom(x => x.EscalatedDate))
				.ForMember(x => x.EscalatedBy, opt => opt.MapFrom(x => x.EscalatedBy))
				.ForMember(x => x.RepeatExisting, opt => opt.MapFrom(x => x.ReturningCustomerState == ReturningCustomerState.NotStarted ? 0 : 1))
				.ForMember(x => x.SalesLead, opt => opt.MapFrom(x => x.SposLead == true ? "Yes" : ""))
				.ForMember(x => x.ProbingQuestions, opt => opt.MapFrom(x => x.ProbingAnswers.Select(s => $"{s.ProbingQuestion.Code}:{s.ProbingQuestionOption.Text}").Concatenate("|", null)))
				.ForMember(dst => dst.FraudConfirmed, opt => opt.MapFrom(x => x.FraudStatus == FraudStatus.FraudConfirmed ? "Yes" : (x.FraudStatus == FraudStatus.NotFraudulent ? "No" : string.Empty)))
				.ForMember(dst => dst.RiskCriteria, opt => opt.MapFrom(x => x.FraudRisks.Replace(',', '|')))
				.ForMember(dst => dst.RiskLevel, opt => opt.MapFrom(x => x.FraudRiskLevel == RiskLevel.High ? RiskLevel.High.ToString() : (x.FraudRiskLevel == RiskLevel.Medium ? RiskLevel.Medium.ToString() : string.Empty)))
				.ForMember(dst => dst.SolverResponse, opt => opt.MapFrom(x => x.SolverMessageCount))
				.ForMember(dst => dst.CustomerResponse, opt => opt.MapFrom(x => x.CustomerMessageCount))
				.ForMember(dst => dst.AvgResponseTime, opt => opt.MapFrom(x => x.AverageSolverResponseTime / 60))
				.ForMember(dst => dst.LongestTime, opt => opt.MapFrom(x => x.SolverMaxResponseTimeInSeconds / 60))
				.ForMember(dst => dst.RatingComment, opt => opt.MapFrom(x => x.AdditionalFeedBack ?? string.Empty))
				.ForMember(dst => dst.IssueType, opt => opt.MapFrom(x => x.TicketCategory != null ? x.TicketCategory.Category.Name : string.Empty))
				.ForMember(x => x.LevelClosed, opt => opt.MapFrom((x, y) => (x.Status, x.Level)
					switch
					{
						(TicketStatusEnum.Closed, TicketLevel.Regular) => "L1",
						(TicketStatusEnum.Closed, TicketLevel.SuperSolver) => "L2",
						_ => null
					}
				))
				.ForMember(x => x.InScopeForCrowd, opt => opt.MapFrom(x => x.ValidTransfer.HasValue ? (x.ValidTransfer.Value == true ? "Yes" : "No") : string.Empty));

			CreateMap<Ticket, TicketAdminExportModel>()
				.IncludeBase<Ticket, ITicketExportContract>()
				.ForMember(x => x.RejectedBy, opt => opt.MapFrom(x => x.RejectedBy.Concatenate("|", null)))
				.ForMember(x => x.CurrentSolver, opt => opt.MapFrom(x => x.AdvocateFullName))
				.ForMember(x => x.LastAcceptedBy, opt => opt.MapFrom( x => x.LastAcceptedBy))
				.ForMember(x => x.LastAbandonedBy, opt => opt.MapFrom( x => x.LastAbandonedBy))
				.ForMember(x => x.Level1Solver, opt => opt.MapFrom(x => x.EscalatedSolver.User.FullName))
				.ForMember(x => x.Level2Solver, opt => opt.MapFrom(x => x.SuperSolver.User.FullName));

			CreateMap<Ticket, TicketClientExportModel>()
				.IncludeBase<Ticket, ITicketExportContract>();

			CreateMap<TicketImport, TicketImportModel>()
				.ForMember(x => x.Failed, opt => opt.MapFrom(x => x.Failures.Count))
				.ForMember(x => x.Imported, opt => opt.MapFrom(x => x.Tickets.Count))
				.ForMember(x => x.Total, opt => opt.MapFrom(x => x.TicketCount))
				.ForMember(x => x.Tags, opt => opt.MapFrom(x => x.Tags.Select(x => x.Tag.Name)))
				.ForMember(x => x.UserFullName, opt => opt.MapFrom(x => x.User.FullName));

			CreateMap<Ticket, ReturningCustomerFlowContext>()
				.ForMember(x => x.TicketId, opt => opt.MapFrom(x => x.Id))
				.ForMember(x => x.Canceled, opt => opt.Ignore());

			CreateMap<Ticket, SerialNumber>()
				.ForMember(x => x.CreatedDate, opt => opt.MapFrom(x => x.CreatedDate))
				.ForMember(x => x.CustomerId, opt => opt.MapFrom(x => x.Customer.Id))
				.ForMember(x => x.SolverId, opt => opt.MapFrom(x => x.AdvocateId))
				.ForMember(x => x.TicketId, opt => opt.MapFrom(x => x.Id))
				.ForMember(x => x.Serial, opt => opt.MapFrom(x => x.Metadata.Where(tmi => tmi.Key.Equals("serialNumber", StringComparison.InvariantCultureIgnoreCase))
				.Select(tmi => tmi.Value).FirstOrDefault()));

			CreateMap<Ticket, EmailInfo>()
				.ForMember(x => x.TicketId, opt => opt.MapFrom(x => x.Id))
				.ForMember(x => x.Value, opt => opt.MapFrom(x => x.Customer.Email))
				.ForMember(x => x.Key, opt => opt.Ignore());

			CreateMap<Ticket, FullNameInfo>()
				.ForMember(x => x.TicketId, opt => opt.MapFrom(x => x.Id))
				.ForMember(x => x.Value, opt => opt.MapFrom(x => x.Customer.FullName))
				.ForMember(x => x.Key, opt => opt.Ignore());

			CreateMap<TrackingDetail, IpInfo>()
				.ForMember(x => x.TicketId, opt => opt.MapFrom(x => x.TicketId))
				.ForMember(x => x.Value, opt => opt.MapFrom(x => x.IpAddress))
				.ForMember(x => x.Key, opt => opt.Ignore());

			CreateMap<IChatActionOptionSideEffect, SideEffectModel>();
			CreateMap<IChatAction, ActionModel>();
			CreateMap<IChatActionOption, ActionOptionModel>();
		}

		private string FlattenTags(List<TicketTag> tags) => tags?.Select(x => x.Tag).Where(t => !t.ParentTagId.HasValue)?.Count() > 0 ? tags.Select(x => x.Tag).Where(t => !t.ParentTagId.HasValue).Select(t => t.Name).Concatenate("|", null) : null;

		private string FlattenSubTags(List<TicketTag> tags) => tags?.Select(x => x.Tag).Where(t => t.ParentTagId.HasValue)?.Count() > 0 ? tags.Select(x => x.Tag).Where(t => t.ParentTagId.HasValue).Select(t => t.Name).Concatenate("|", null) : null;

		private List<TagModel> MapToTagModel(List<Tag> tags)
		{
			var subTagIds = tags.Where(t => t.ParentTagId.HasValue).Select(t => t.Id).ToHashSet();
			return tags.Where(x => !x.ParentTagId.HasValue).Select(t => new TagModel
			{
				Id = t.Id,
				Name = t.Name,
				Action = t.Action,
				SubTags = t.SubTags?.Where(st => subTagIds.Contains(st.Id))
						.Select(r => new TagModel
						{
							Id = r.Id,
							Name = r.Name,
							Action = r.Action,
						})?.ToList()
			}).ToList();
		}
	}
}