using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Customer;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Services
{
	public class CustomerService : ICustomerService
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		private readonly ITicketUrlService _ticketUrlService;
		private readonly IJwtService _jwtService;

		private readonly IBrandRepository _brandRepository;

		public CustomerService(IUserRepository userRepository,
		ITicketRepository ticketRepository, ITicketUrlService ticketUrlService,
		IMapper mapper, IJwtService jwtService, IBrandRepository brandRepository)
		{
			_ticketRepository = ticketRepository;
			_userRepository = userRepository;
			_mapper = mapper;
			_ticketUrlService = ticketUrlService;
			_jwtService = jwtService;
			_brandRepository = brandRepository;
		}

		/// <inheritdoc/>
		public async Task<CustomerModel> GetTickets(string customerEmail)
		{
			var customer = await _userRepository.GetByEmail(customerEmail);

			if (customer != null)
			{
				var tickets = _mapper.Map<List<CustomerTicketModel>>(await _ticketRepository.GetCustomerTickets(customer.Id));
				foreach (var t in tickets)
				{
					t.ChatUrl = await _ticketUrlService.GetChatUrl(t.Id, false);
					t.Question = t.Question.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").Truncate(30, true);
				}

				var customerTickets = new CustomerModel
				{
					FirstName = customer.FirstName,
					LastName = customer.LastName,
					Email = customer.Email,
					Tickets = tickets,
				};

				return customerTickets;
			}
			else
			{
				return null;
			}
		}
	}
}