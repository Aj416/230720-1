using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Services
{
	public class MaintenanceService : IMaintenanceService
	{
		private readonly IAuthenticationService _authenticationService;
		private readonly ILogger<MaintenanceService> _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ITicketRepository _ticketRepository;
		private readonly ITimestampService _timestampService;
		private readonly IMediatorHandler _mediator;

		public MaintenanceService(
			IAuthenticationService authenticationService,
			ILogger<MaintenanceService> logger,
			IUnitOfWork unitOfWork,
			IMapper mapper,
			IMediatorHandler mediator,
			ITicketRepository ticketRepository,
			ITimestampService timestampService)
		{
			_authenticationService = authenticationService;
			_logger = logger;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_mediator = mediator;
			_ticketRepository = ticketRepository;
			_timestampService = timestampService;
		}
		public Task MarkAllEmailsAsVerified() => _authenticationService.MarkAllEmailsAsVerified();

	}
}