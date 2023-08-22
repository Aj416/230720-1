using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Application.Commands.Fraud;
using Tigerspike.Solv.Application.Models.Fraud;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Application.EventHandlers
{
	public class FraudDetectEventHandler :
		INotificationHandler<TicketCreatedEvent>,
		INotificationHandler<TicketAcceptedEvent>,
		INotificationHandler<TicketSolvedEvent>,
		INotificationHandler<TicketClosedEvent>,
		INotificationHandler<TicketCsatSetEvent>,
		INotificationHandler<TicketReopenedEvent>
	{
		private const string CreateTicketCommand = "CreateTicketCommand";
		private readonly ITicketRepository _ticketRepository;
		private readonly IUserRepository _userRepository;
		private readonly ITrackingDetailRepository _trackingDetailRepository;
		private readonly IMapper _mapper;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;

		public FraudDetectEventHandler(IUserRepository userRepository,
			ITicketRepository ticketRepository,
			ITrackingDetailRepository trackingDetailRepository,
			IMapper mapper,
			IBus bus, IOptions<BusOptions> busOptions)
		{
			_ticketRepository = ticketRepository;
			_userRepository = userRepository;
			_trackingDetailRepository = trackingDetailRepository;
			_mapper = mapper;
			_bus = bus ??
					   throw new ArgumentNullException(nameof(bus));
			_busOptions = busOptions.Value;
		}

		private async Task<List<ISerialNumber>> GetSerialNumberInfoForLastWeek(Guid ticketId, Guid? customerId, Guid? solverId)
		{
			var result = new List<ISerialNumber>();
			var serialNumber = await _ticketRepository.GetSerialNumber(ticketId);
			if (!string.IsNullOrEmpty(serialNumber) && customerId.HasValue)
			{
				result.AddRange(await _ticketRepository.GetAllAsync<SerialNumber>(_mapper,
					predicate: x => x.Customer.Id != customerId.Value && x.Metadata.Any(tmi => tmi.Value.Equals(serialNumber, StringComparison.InvariantCultureIgnoreCase)) &&
					x.CreatedDate > DateTime.UtcNow.AddDays(-7).Date,
					include: src => src.Include(t => t.Metadata).Include(t => t.Customer)));
			}

			if (!string.IsNullOrEmpty(serialNumber) && solverId.HasValue)
			{
				result.AddRange(await _ticketRepository.GetAllAsync<SerialNumber>(_mapper,
					predicate: x => x.AdvocateId == solverId && x.Metadata.Any(tmi => tmi.Value.Equals(serialNumber, StringComparison.InvariantCultureIgnoreCase)) &&
					x.CreatedDate > DateTime.UtcNow.AddDays(-7).Date,
					include: src => src.Include(t => t.Metadata)));
			}

			return result;
		}

		private async Task<List<ISerialNumber>> GetSerialNumberInfoForLastThreeDays(Guid ticketId)
		{
			var result = new List<ISerialNumber>();
			var serialNumber = await _ticketRepository.GetSerialNumber(ticketId);
			if (!string.IsNullOrEmpty(serialNumber))
			{
				result.AddRange(await _ticketRepository.GetAllAsync<SerialNumber>(_mapper,
					predicate: x => x.Metadata.Any(tmi => tmi.Value.Equals(serialNumber, StringComparison.InvariantCultureIgnoreCase)) &&
					x.CreatedDate > DateTime.UtcNow.AddDays(-3).Date,
					include: src => src.Include(t => t.Metadata).Include(t => t.Customer)));
			}

			return result;
		}

		private async Task<List<ICustomer>> GetCustomerInfoForLastDay(Guid customerId)
		{
			var result = new List<ICustomer>();
			var userInfo = await _userRepository.GetFirstOrDefaultAsync(predicate: u => u.Id == customerId);

			result.AddRange(await _ticketRepository.GetAllAsync<EmailInfo>(_mapper,
				predicate: t => t.Customer.Email == userInfo.Email &&
				t.CreatedDate > DateTime.UtcNow.AddDays(-1).Date,
				include: src => src.Include(t => t.Customer)));

			result.AddRange(await _ticketRepository.GetAllAsync<FullNameInfo>(_mapper,
				predicate: t => t.Customer.LastName == userInfo.LastName &&
				t.Customer.FirstName == userInfo.FirstName &&
				t.CreatedDate > DateTime.UtcNow.AddDays(-1).Date,
				include: src => src.Include(t => t.Customer)));

			return result;
		}

		private async Task<List<ICustomer>> GetIpInfoForLastDay(string ipAddress, string messageType)
		{
			var result = new List<ICustomer>();

			result.AddRange(await _trackingDetailRepository.GetAllAsync<IpInfo>(_mapper,
				predicate: t => t.IpAddress == ipAddress &&
				t.CreatedDate > DateTime.UtcNow.AddDays(-1).Date && t.Event.Equals(messageType, StringComparison.InvariantCultureIgnoreCase)));

			return result;
		}

		private async Task<List<int>> GetAdvocateResponseTimeForLastTwoDays(Guid? advocateId)
		{
			var result = new List<int>();
			if (advocateId.HasValue)
			{
				var tickets = await _ticketRepository.GetAllAsync(predicate: t => t.AdvocateId == advocateId &&
				t.Status == TicketStatusEnum.Closed &&
				t.ClosedDate > DateTime.UtcNow.AddDays(-2).Date,
				include: src => src.Include(t => t.StatusHistory)
			);

				result.AddRange(tickets.Select(t => new
				{
					ResponseTime =
						(int?)(t.GetClosedDateForAdvocate(t.AdvocateId) - t.GetAcceptedDateByAdvocate(t.AdvocateId))?.TotalMinutes
				})
				.Where(x => x.ResponseTime.HasValue)
				.Select(s => s.ResponseTime.Value).ToList());
			}

			return result;
		}

		public async Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
		{
			if (!notification.IsPractice && notification.TransportType != TicketTransportType.Import)
			{
				var serialInfoForLastWeek = await GetSerialNumberInfoForLastWeek(notification.TicketId, notification.CustomerId, null);
				var serialInfoForLastThreeDays = await GetSerialNumberInfoForLastThreeDays(notification.TicketId);
				var customerInfoForLastDay = await GetCustomerInfoForLastDay(notification.CustomerId);
				var ticketDetail = await _ticketRepository.GetFullTicket(p => p.Id == notification.TicketId);
				var ipAddress = ticketDetail.TrackingHistory.Where(th => th.UserId == notification.CustomerId).Select(th => th.IpAddress).FirstOrDefault();
				var ipInfoForLastDay = await GetIpInfoForLastDay(ipAddress, CreateTicketCommand);
				customerInfoForLastDay.AddRange(ipInfoForLastDay);
				var metadata = new Dictionary<string, string>(ticketDetail.Metadata?.OrderBy(x => x.Order ?? int.MaxValue).Select(x => KeyValuePair.Create(x.Key, x.Value)));

				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Fraud}"));
				await endpoint.Send<IDetectFraudCommand>(new DetectFraudCommand(serialInfoForLastWeek, serialInfoForLastThreeDays, customerInfoForLastDay, (int)ticketDetail.Status, (int)ticketDetail.Level,
					ticketDetail.CreatedDate, ticketDetail.Id, notification.BrandId, ticketDetail.Brand.Name, notification.CustomerId, ipAddress, null, metadata, ticketDetail.Customer.FirstName, ticketDetail.Customer.LastName, ticketDetail.Customer.Email, ticketDetail.Question), cancellationToken);
			}
		}

		public async Task Handle(TicketAcceptedEvent notification, CancellationToken cancellationToken)
		{
			if (!notification.IsPractice && notification.TransportType != TicketTransportType.Import)
			{
				var serialInfoForLastWeek = await GetSerialNumberInfoForLastWeek(notification.TicketId, null, notification.AdvocateId);
				var ticketDetail = await _ticketRepository.GetFullTicket(p => p.Id == notification.TicketId);
				var customerIp = ticketDetail.TrackingHistory.Where(th => th.UserId == ticketDetail.Customer.Id).Select(th => th.IpAddress).FirstOrDefault();
				var solverIp = ticketDetail.TrackingHistory.Where(th => th.UserId == notification.AdvocateId).Select(th => th.IpAddress).FirstOrDefault();
				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Fraud}"));
				await endpoint.Send<IDetectFraudCommand>(new DetectFraudCommand(serialInfoForLastWeek, (int)ticketDetail.Status, (int)ticketDetail.Level,
					notification.TicketId, ticketDetail.Advocate.User.FullName, notification.BrandId, ticketDetail.Brand.Name, ticketDetail.Customer.Id, customerIp, solverIp), cancellationToken);
			}
		}

		public async Task Handle(TicketSolvedEvent notification, CancellationToken cancellationToken)
		{
			if (!notification.IsPractice && notification.TransportType != TicketTransportType.Import)
			{
				var ticketDetail = await _ticketRepository.GetFullTicket(p => p.Id == notification.TicketId);
				var solvedDate = ticketDetail.GetSolvedDateByAdvocate(ticketDetail.AdvocateId ?? null);

				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Fraud}"));
				await endpoint.Send<IDetectFraudCommand>(new DetectFraudCommand((int)ticketDetail.Status, (int)ticketDetail.Level,
					notification.TicketId, ticketDetail.Advocate.User.FullName, notification.BrandId, ticketDetail.Brand.Name, ticketDetail.Customer.Id), cancellationToken);
			}
		}

		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken)
		{
			if (!notification.IsPractice && notification.TransportType != TicketTransportType.Import)
			{
				var ticketDetail = await _ticketRepository.GetFullTicket(p => p.Id == notification.TicketId);
				var acceptedDate = ticketDetail.GetAcceptedDateByAdvocate(ticketDetail.AdvocateId ?? null);
				var solvedDate = ticketDetail.GetSolvedDateByAdvocate(ticketDetail.AdvocateId ?? null);
				var closedDate = ticketDetail.GetClosedDateForAdvocate(ticketDetail.AdvocateId ?? null);
				var responseTimes = await GetAdvocateResponseTimeForLastTwoDays(ticketDetail.AdvocateId ?? null);
				var closeTime = (int?)(closedDate - solvedDate)?.TotalSeconds;
				var currentResponseTime = (int?)(closedDate - acceptedDate)?.TotalMinutes;
				var closedByCustomer = ticketDetail.ClosedBy == Domain.Enums.ClosedBy.Customer;
				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Fraud}"));
				await endpoint.Send<IDetectFraudCommand>(new DetectFraudCommand(currentResponseTimeInMinutes: currentResponseTime, responseTimes: responseTimes, closeTime: closeTime, notification.TicketId, csat: ticketDetail.Csat, closedByCustomer, (int)ticketDetail.Status,
					(int)ticketDetail.Level, advocateName: ticketDetail?.Advocate?.User?.FullName, notification.BrandId, ticketDetail.Brand.Name, notification.CustomerId), cancellationToken);
			}
		}

		public async Task Handle(TicketCsatSetEvent notification, CancellationToken cancellationToken)
		{
			if (!notification.IsPractice && notification.TransportType != TicketTransportType.Import)
			{
				var ticketDetail = await _ticketRepository.GetFullTicket(p => p.Id == notification.TicketId);
				var closedByCustomer = ticketDetail.ClosedBy == Domain.Enums.ClosedBy.Customer;

				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Fraud}"));
				await endpoint.Send<IDetectFraudCommand>(new DetectFraudCommand(currentResponseTimeInMinutes: null, responseTimes: null, closeTime: null, notification.TicketId, csat: ticketDetail.Csat, closedByCustomer, (int)ticketDetail.Status,
					(int)ticketDetail.Level, advocateName: ticketDetail.Advocate.User.FullName, notification.BrandId, ticketDetail.Brand.Name, ticketDetail.Customer.Id), cancellationToken);
			}
		}

		public async Task Handle(TicketReopenedEvent notification, CancellationToken cancellationToken)
		{
			if (!notification.IsPractice && notification.TransportType != TicketTransportType.Import)
			{
				var serialInfoForLastWeek = await GetSerialNumberInfoForLastWeek(notification.TicketId, null, notification.AdvocateId);
				var ticketDetail = await _ticketRepository.GetFullTicket(p => p.Id == notification.TicketId);
				var customerIp = ticketDetail.TrackingHistory.Where(th => th.UserId == ticketDetail.Customer.Id).Select(th => th.IpAddress).FirstOrDefault();
				var solverIp = ticketDetail.TrackingHistory.Where(th => th.UserId == notification.AdvocateId).Select(th => th.IpAddress).FirstOrDefault();
				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Fraud}"));
				await endpoint.Send<IDetectFraudCommand>(new DetectFraudCommand(serialInfoForLastWeek, (int)ticketDetail.Status, (int)ticketDetail.Level,
					notification.TicketId, ticketDetail.Advocate.User.FullName, notification.BrandId, ticketDetail.Brand.Name, ticketDetail.Customer.Id, customerIp, solverIp), cancellationToken);
			}
		}
	}
}