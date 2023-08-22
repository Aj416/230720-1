using System;
using AutoMapper;
using Tigerspike.Solv.Chat.Domain;
using Tigerspike.Solv.Messaging.Chat;
using Tigerspike.Solv.Services.Chat.Application.Models;
using Action = Tigerspike.Solv.Chat.Domain.Action;

namespace Tigerspike.Solv.Services.Chat.Application.AutoMapper
{
	public class ChatProfile : Profile
	{
		public ChatProfile()
		{
			// Chat related mappings
			CreateMap<Message, MessageModel>()
				.ForMember(m => m.AuthorId, opt => opt.MapFrom(src => Guid.Parse(src.AuthorId)))
				.ForMember(dst => dst.Message, opt => opt.MapFrom(src => src.Content))
				.ForMember(dst => dst.ClientMessageId, opt => opt.Ignore())
				.ForMember(dst => dst.UserFirstName, opt => opt.Ignore())
				.ForMember(dst => dst.ThreadId, opt => opt.Ignore())
				// File
				.ForMember(dst => dst.MimeType, opt => opt.Ignore())
				.ForMember(dst => dst.FileName, opt => opt.Ignore())
				.ForMember(dst => dst.Path, opt => opt.Ignore())
				.ForMember(dst => dst.MessageFileTypeId, opt => opt.Ignore())
				.AfterMap((src, dst) =>
				{
					if (src.File != null)
					{
						dst.MimeType = src.File.ContentType;
						dst.FileName = src.File.FileName;
						dst.Path = $"https://{src.File.BucketName}.s3.amazonaws.com/{src.File.Key}";
						dst.MessageFileTypeId = src.File.FileType;
					}
				});

			CreateMap<Conversation, ConversationModel>();


			CreateMap<ActionModel, Action>()
				.ForMember(dst => dst.HashKey, opt => opt.Ignore())
				.ForMember(dst => dst.RangeKey, opt => opt.Ignore())
				.ForMember(dst => dst.RecordType, opt => opt.Ignore());


			CreateMap<ActionOptionModel, ActionOption>()
				.ForMember(dst => dst.HashKey, opt => opt.Ignore())
				.ForMember(dst => dst.RangeKey, opt => opt.Ignore())
				.ForMember(dst => dst.RecordType, opt => opt.Ignore());

			CreateMap<SideEffectModel, SideEffect>()
				.ForMember(dst => dst.Id, opt => opt.Ignore());

			CreateMap<Action, ActionModel>();
			CreateMap<ActionOption, ActionOptionModel>();
			CreateMap<SideEffect, SideEffectModel>();

			CreateMap<SideEffectModel, IChatActionOptionSideEffect>()
				.ConstructUsing( x =>
					new IntegrationEvents.ChatActionOptionSideEffect((int) x.Effect, x.Value));
			
			CreateMap<ActionOptionModel, IChatActionOption>()
				.ConstructUsing(x =>
					new IntegrationEvents.ChatActionOption(x.Label, x.Value, x.IsSelected, x.IsSuggested));
		}
	}
}
