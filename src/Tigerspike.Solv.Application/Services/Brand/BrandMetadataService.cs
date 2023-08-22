using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Services
{
	/// <summary>
	/// The metadata service of the brand
	/// </summary>
	public class BrandMetadataService : IBrandMetadataService
	{
		#region Private Properties

		/// <summary>
		/// The metadata access repository
		/// </summary>
		private readonly IBrandMetadataAccessRepository _brandMetadataAccessRepository;

		/// <summary>
		/// The metadata routing config repository
		/// </summary>
		private readonly IBrandMetadataRoutingConfigRepository _brandMetadataRoutingConfigRepository;
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="brandMetadataAccessRepository"></param>
		/// <param name="brandMetadataRoutingConfigRepository"></param>
		public BrandMetadataService(IBrandMetadataAccessRepository brandMetadataAccessRepository, IBrandMetadataRoutingConfigRepository brandMetadataRoutingConfigRepository)
		{
			_brandMetadataAccessRepository = brandMetadataAccessRepository;
			_brandMetadataRoutingConfigRepository = brandMetadataRoutingConfigRepository;
		}
		#endregion

		#region Public Methods

		/// <inheritdoc />
		public async Task RouteToLevel(Ticket ticket, IEnumerable<TicketMetadataItem> metadata)
		{
			var metadataRoutingConfig = await _brandMetadataRoutingConfigRepository.GetRoutingConfig(ticket.BrandId);

			if (metadataRoutingConfig != null)
			{
				var routeMetadata = metadata.FirstOrDefault(x => x.Key.Equals(metadataRoutingConfig.Field, StringComparison.InvariantCultureIgnoreCase));
				if (routeMetadata != null && routeMetadata.Value.Equals(metadataRoutingConfig.Value, StringComparison.InvariantCultureIgnoreCase))
				{
					ticket.Level = metadataRoutingConfig.RouteTo;
					ticket.EscalatedDate = ticket.CreatedDate;
					ticket.EscalationReason = TicketEscalationReason.Urgent;
				}
			}
		}

		/// <inheritdoc />
		public async Task FilterMetadata(Ticket ticket, AccessLevel level)
		{
			if (level != AccessLevel.Admin)
			{
				var brandMetadataAccess = await _brandMetadataAccessRepository.GetAccessDetails(ticket.BrandId);
				brandMetadataAccess.ForEach(metadata =>
				{
					var ticketMetadata = ticket.Metadata.FirstOrDefault(x => x.Key.Equals(metadata.Field, StringComparison.InvariantCultureIgnoreCase));
					if (ticketMetadata != null && ticket.Level != metadata.Level)
					{
						ticket.Metadata.Remove(ticketMetadata);
					}
				});
			}
		}

		/// <inheritdoc />
		public async Task FilterMetadata(IEnumerable<Ticket> tickets)
		{
			var brandIds = tickets.Select(x => x.BrandId).Distinct().ToList();

			var brandAccesses = new Dictionary<Guid, List<BrandMetadataAccess>>();

			foreach (var brnd in brandIds)
			{
				var brandMetadataAccess = await _brandMetadataAccessRepository.GetAccessDetails(brnd);
				brandAccesses.Add(brnd, brandMetadataAccess);
			}

			tickets.ToList().ForEach(ticket =>
			{
				var metadataAccess = brandAccesses[ticket.BrandId];
				metadataAccess.ForEach(metadata =>
				{
					var ticketMetadata = ticket.Metadata.FirstOrDefault(x => x.Key.Equals(metadata.Field, StringComparison.InvariantCultureIgnoreCase));
					if (ticketMetadata != null && ticket.Level != metadata.Level)
					{
						ticket.Metadata.Remove(ticketMetadata);
					}
				});
			});
		}
		#endregion
	}
}