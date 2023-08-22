using System;
using System.Linq;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Services.Chat.Application.Commands
{
	public class DeleteBrandWhitelistPhrasesCommand : Command<string[]>
	{
		/// <summary>
		/// The brand id of the whitelistPhrases
		/// </summary>
		/// <value></value>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The list of WhitelistPhrases to be assigned the brand.
		/// </summary>
		public string[] WhitelistPhrases { get; set; }

		public DeleteBrandWhitelistPhrasesCommand(Guid brandId, string[] whitelistPhrases)
		{

			BrandId = brandId;
			WhitelistPhrases = whitelistPhrases;

		}
		public override bool IsValid() => BrandId != Guid.Empty && WhitelistPhrases.Any();


	}
}