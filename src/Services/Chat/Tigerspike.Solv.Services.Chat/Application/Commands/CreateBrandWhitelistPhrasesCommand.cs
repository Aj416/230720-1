using System;
using System.Linq;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Services.Chat.Application.Commands
{
	public class CreateBrandWhitelistPhrasesCommand : Command<string[]>
	{
		/// <summary>
		/// The brand Id
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// List of white-listed phrases
		/// </summary>
		public string[] WhitelistPhrases { get; set; }

		public CreateBrandWhitelistPhrasesCommand(Guid brandId, string[] whitelistphrases)
		{
			BrandId = brandId;
			WhitelistPhrases = whitelistphrases;
		}

		public override bool IsValid() => BrandId != Guid.Empty && WhitelistPhrases.Any();
	}
}