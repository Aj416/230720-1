using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models.Brand;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface ITicketAutoResponseService
	{
		Task CancelResponses(Guid brandId, Guid ticketId, BrandAdvocateResponseType[] responseTypes);
		Task<IEnumerable<BrandAdvocateResponseConfig>> GetResponses(Guid brandId, BrandAdvocateResponseType[] responseTypes, int? abandonedCount = null, bool? isAutoAbandoned = null, TicketEscalationReason? escalationReason = null);
		Task SendResponses(Guid brandId, Guid ticketId, BrandAdvocateResponseType[] responseTypes, UserType userType = UserType.SolvyBot, Guid? authorId = null, BrandResponseTemplateModel model = null, int? abandonedCount = null, bool? isAutoAbandoned = null, TicketEscalationReason? escalationReason = null, TimeSpan? advanceScheduleBy = null);
	}
}
