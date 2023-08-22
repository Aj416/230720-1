using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Application.Models.Client;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Interfaces
{
	public class ClientService : IClientService
	{
		private readonly IClientRepository _clientRepository;
		private readonly IMediatorHandler _mediatorHandler;
		private readonly IMapper _mapper;

		public ClientService(
			IClientRepository clientRepository,
			IMediatorHandler mediatorHandler,
			IMapper mapper)
		{
			_clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
			_mediatorHandler = mediatorHandler;
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		/// <inheritdoc/>
		public async Task<ClientModel> GetClient(Guid clientId)
		{
			var client = await _clientRepository.GetFirstOrDefaultAsync(i => i,
				pr => pr.Id == clientId,
				include: src => src.Include(u => u.User).Include(b => b.Brand));

			return _mapper.Map<ClientModel>(client);
		}

		/// <inheritdoc/>
		public async Task<Guid?> CreateClient(Guid brandId, string firstName, string lastName, string email, string phone, string password) =>
			await _mediatorHandler.SendCommand(new CreateClientIdentityCommand(Guid.NewGuid(), brandId, firstName, lastName, email, phone, password));
	}
}
