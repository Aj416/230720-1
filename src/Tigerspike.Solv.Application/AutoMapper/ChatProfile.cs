using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Tigerspike.Solv.Application.Models.Chat;
using Tigerspike.Solv.Domain.DTOs;

namespace Tigerspike.Solv.Application.AutoMapper
{
	public class ChatProfile : Profile
	{
		public ChatProfile()
		{
			CreateMap<TicketMessageModel, MessageAddModel>()
				.ForMember(x => x.SenderType, opt => opt.MapFrom(o => (int) o.SenderType))
				.ForMember(x => x.RelevantTo, opt => opt.MapFrom(o => o.RelevantTo.Select(x => (int)x).ToArray()))
				.ForMember(x => x.MessageType, opt => opt.MapFrom(o => (int?) o.MessageType))
				.ForMember(x => x.ClientMessageId, opt => opt.Ignore())
				.IgnoreAllPropertiesWithAnInaccessibleSetter()
				.IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
		}
	}
}
