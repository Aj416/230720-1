using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Application.Refit;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.IdentityVerification;

namespace Tigerspike.Solv.Application.Services
{
	public class IdentityVerificationService : IIdentityVerificationService
	{
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;
		private readonly IIdentityVerificationClient _identityVerificationClient;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly ILogger<IdentityVerificationService> _logger;
		private readonly IMediatorHandler _mediator;

		public IdentityVerificationService(
			IIdentityVerificationClient identityVerificationClient,
			IAdvocateRepository advocateRepository,
			IBus bus,
			IMediatorHandler mediator,
			IOptions<BusOptions> busOptions,
			ILogger<IdentityVerificationService> logger)
		{
			_identityVerificationClient = identityVerificationClient;
			_advocateRepository = advocateRepository;
			_bus = bus;
			_mediator = mediator;
			_busOptions = busOptions.Value;
			_logger = logger;
		}

		/// <inheritdoc />
		public async Task CreateCheck(Guid advocateId)
		{
			var advocate =
				await _advocateRepository.GetSingleOrDefaultAsync(
					a => new { a.Id, FirstName = a.User.FirstName, LastName = a.User.LastName, a.IdentityApplicantId },
					a => a.Id == advocateId, include: i => i.Include(a => a.User));

			if (advocate != null)
			{
				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.IdentityVerification}"));

				// Temporary solution for the requirement of a synchronous response
				// Must be removed to leverage the microservices directly

				await _mediator.SendCommand(
					new SetIdentityCheckDetailsCommand(advocate.Id, "in progress",
						"in progress"));

				await endpoint.Send<ICreateIdentityCheckCommand>(new
				{
					AdvocateId = advocate.Id,
					ApplicantId = advocate.IdentityApplicantId,
					FirstName = advocate.FirstName,
					LastName = advocate.LastName
				});
			}
		}

		/// <inheritdoc />
		public async Task<string> GenerateSdkToken(Guid advocateId)
		{
			var advocate =
				await _advocateRepository.GetSingleOrDefaultAsync(
					a => new {a.Id, FirstName = a.User.FirstName, LastName = a.User.LastName, a.IdentityApplicantId},
					a => a.Id == advocateId);

			if (advocate != null)
			{
				return await _identityVerificationClient.GenerateSdkToken(advocateId, advocate.IdentityApplicantId,
					advocate.FirstName, advocate.LastName);
			}

			return null;
		}

		public async Task ConsumeIdentityCheckWebhook(HttpRequest request)
		{
			await _identityVerificationClient.ConsumeIdentityCheckWebhook(await GetPayload(request), GetSignature(request));
		}

		private static string GetSignature(HttpRequest request) =>
			request.Headers.TryGetValue("x-sha2-signature", out var signatureHeaders)
				? signatureHeaders.FirstOrDefault()
				: null;

		private static async Task<string> GetPayload(HttpRequest request)
		{
			using var reader = new StreamReader(request.Body);
			return await reader.ReadToEndAsync();
		}
	}
}