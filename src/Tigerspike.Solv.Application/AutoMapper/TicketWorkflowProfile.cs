using AutoMapper;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Domain.Commands.Chat;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.AutoMapper
{
	/// <summary>
	/// The ticket workflow profile
	/// </summary>
	public class TicketWorkflowProfile : Profile
	{
		public TicketWorkflowProfile()
		{
			// The mappings for the workflow initialization
			CreateMap<CreateTicketWorkflowModel, CreateTicketWorkflowInitializationModel>()
				.IgnoreAllPropertiesWithAnInaccessibleSetter();

			// The mappings for the sending message
			CreateMap<CreateTicketWorkflowModel, SendInitialChatMessageCommand>()
				.ForMember(x => x.CreatedDate, opt => opt.MapFrom(o => o.Timestamp))
				.ForMember(x => x.Timestamp, opt => opt.Ignore())
				.ForMember(x => x.Messages, opt => opt.Ignore())
				.IgnoreAllPropertiesWithAnInaccessibleSetter();

			// The mappings for the notifying status changed
			CreateMap<CreateTicketWorkflowModel, TicketStatusChangedNotificationModel>()
				.ForMember(x => x.TicketId, opt => opt.MapFrom(o => o.TicketId))
				.ForMember(x => x.ToStatus, opt => opt.MapFrom(o => TicketStatusEnum.New))
				.ForMember(x => x.CustomerId, opt => opt.MapFrom(o => o.CustomerId))
				.ForAllOtherMembers(x => x.Ignore());

			CreateMap<CompleteTicketWorkflowModel, TicketStatusChangedNotificationModel>()
				.ForMember(x => x.TicketId, opt => opt.MapFrom(o => o.TicketId))
				.ForMember(x => x.ToStatus, opt => opt.MapFrom(o => TicketStatusEnum.Closed))
				.ForMember(x => x.AdvocateId, opt => opt.MapFrom(o => o.AdvocateId))
				.ForMember(x => x.AdvocateFirstName, opt => opt.MapFrom(o => o.AdvocateFirstName))
				.ForMember(x => x.AdvocateCsat, opt => opt.MapFrom(o => o.AdvocateCsat))
				.ForMember(x => x.CustomerId, opt => opt.MapFrom(o => o.CustomerId))
				.ForAllOtherMembers(x => x.Ignore());

			// The mappings for the notification
			CreateMap<CreateTicketWorkflowModel, CreateTicketNotificationModel>()
				.IgnoreAllPropertiesWithAnInaccessibleSetter();

			
		}
	}
}
