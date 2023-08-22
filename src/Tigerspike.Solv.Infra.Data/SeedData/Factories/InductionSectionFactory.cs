using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Induction;

namespace Tigerspike.Solv.Infra.Data.SeedData.Factories
{
	public class InductionSectionFactory
	{
		public static List<Section> GetList()

		{
			return new List<Section>
			{
				new Section(Guid.NewGuid(), "Identity and values", true, Brand.DemoBrandId, 1, new List<SectionItem>
				{
					new SectionItem(Guid.NewGuid(), "About CompanyOne", "https://otro.com/gb/about", true, 1),
					new SectionItem(Guid.NewGuid(), "Introduction to CompanyOne", "https://otro.com/gb/", true, 2),
					new SectionItem(Guid.NewGuid(), "Community guidelines", "https://www.otro.com/gb/terms-of-use#community-guidelines", true, 3)
				}),
				new Section(Guid.NewGuid(),"Product knowledge", true, Brand.DemoBrandId, 2, new List<SectionItem>
				{
					new SectionItem(Guid.NewGuid(), "What we do", "https://help.otro.com/hc/en-gb/articles/360016033411-What-We-Do", true, 1),
					new SectionItem(Guid.NewGuid(), "App technical support FAQ", "https://help.otro.com/hc/en-gb/sections/360002264651-USING-OTRO", true, 2),
					new SectionItem(Guid.NewGuid(), "Payments and accounts", "https://help.otro.com/hc/en-gb/sections/360002264831-MY-ACCOUNT-AND-PAYMENT-", true, 3),
					new SectionItem(Guid.NewGuid(), "How to Translate in Chrome", "https://support.google.com/chrome/answer/173424", true, 4),
					new SectionItem(Guid.NewGuid(), "Download Google Translate for Chrome", "https://chrome.google.com/webstore/detail/google-translate/aapbdbdomjkkjkaonfhkkikfgjllcleb", true, 5)
				})
			};
		}
	}
}