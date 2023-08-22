using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Chat.Domain;
using Tigerspike.Solv.Chat.Infrastructure.Repositories;

namespace Tigerspike.Solv.Services.Chat.Application.Commands
{
	public class MessageWhiteListCommandHandler :
		IRequestHandler<CreateBrandWhitelistPhrasesCommand, string[]>,
		IRequestHandler<DeleteBrandWhitelistPhrasesCommand, string[]>
	{
		private readonly IMessageWhitelistRepository _messageWhitelistRepository;
		private readonly ICachedMessageWhitelistRepository _cachedMessageWhitelistRepository;

		public MessageWhiteListCommandHandler(
			IMessageWhitelistRepository messageWhitelistRepository,
			ICachedMessageWhitelistRepository cachedMessageWhitelistRepository)
		{
			_messageWhitelistRepository = messageWhitelistRepository;
			_cachedMessageWhitelistRepository = cachedMessageWhitelistRepository;
		}

		public Task<string[]> Handle(CreateBrandWhitelistPhrasesCommand request,
			CancellationToken cancellationToken)
		{
			var messages = _messageWhitelistRepository.GetList(request.BrandId);

			var added = new List<string>();

			foreach (var phrase in request.WhitelistPhrases)
			{
				if (!messages.Any(m => m.Phrase == phrase))
				{
					_messageWhitelistRepository.AddOrUpdateMessage(new MessageWhitelist(request.BrandId, phrase));

					added.Add(phrase);
				}
			}

			// Invalidate the cache
			_cachedMessageWhitelistRepository.Invalidate(request.BrandId);

			return Task.FromResult(added.ToArray());
		}

		public Task<string[]> Handle(DeleteBrandWhitelistPhrasesCommand request, CancellationToken cancellationToken)
		{
			var messages = _messageWhitelistRepository.GetList(request.BrandId);

			var deleted = new List<string>();

			foreach (var phrase in request.WhitelistPhrases)
			{
				var message = messages.FirstOrDefault(m => m.Phrase == phrase);

				if (message != null)
				{
					_messageWhitelistRepository.DeleteMessage(request.BrandId, message.MessageId);

					deleted.Add(phrase);
				}
			}

			// Invalidate the cache
			_cachedMessageWhitelistRepository.Invalidate(request.BrandId);

			return Task.FromResult(deleted.ToArray());
		}
	}
}