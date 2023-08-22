using System.Collections.Generic;
using Tigerspike.Solv.Chat.Application.ChatCensor.Strategies;
using Tigerspike.Solv.Chat.Domain;
using Xunit;

namespace Tigerspike.Solv.Chat.Tests.ChatCensor
{
	public class ChatCensorTest
	{
		//Visa (incl. VPay)
		[InlineData("4026 4123 4123 4", "****************")] // 13 digits
		[InlineData("4026 4123 4123 43", "*****************")]
		[InlineData("4026 4123 4123 433", "******************")]
		[InlineData("4026 4123 4123 4333", "*******************")]
		[InlineData("4026 4123 4123 43331", "********************")]
		[InlineData("4026 4123 4123 433312", "*********************")]
		[InlineData("4930 8060 5917 3016680", "**********************")] // 19 digits

		//Visa electron 16 digits starts with 4026, 417500, 4405, 4508, 4844, 4913, 4917	
		[InlineData("4026 4123 4123 4334", "*******************")]
		[InlineData("4175 0023 4123 4333", "*******************")]
		[InlineData("4405 4123 4123 4333", "*******************")]
		[InlineData("4508 4123 4123 4333", "*******************")]
		[InlineData("4844 4123 4123 4333", "*******************")]
		[InlineData("4913 4123 4123 4333", "*******************")]
		[InlineData("4917 4123 4123 4333", "*******************")]
		//American Express starts with 34, 37
		[InlineData("3426 4123 4123 4333123", "**********************")]
		[InlineData("3726 4123 4123 4333123", "**********************")]
		//China UnionPay 16-19 digits starts with 62
		[InlineData("6226 4123 4123 4333", "*******************")]
		[InlineData("6226 4123 4123 43331", "********************")]
		[InlineData("6226 4123 4123 433312", "*********************")]
		[InlineData("6226 4123 4123 4333123", "**********************")]
		//No spaces
		[InlineData("4123412341234333", "****************")]
		//Dashes
		[InlineData("4123-4123-4123-4333", "*******************")]
		[InlineData("4123-4123 4123-4333", "*******************")]
		[InlineData("4123-4123 4123 4333", "*******************")]
		[InlineData("4123 4123-4123 4333", "*******************")]
		//With text
		[InlineData("My card number is 4123-4123-4123-4333", "My card number is *******************")]
		[InlineData("card number: 4123-4123-4123-4333 master", "card number: ******************* master")]
		//Shouldn't be censored.
		[InlineData("4123 SomeText 4123-4123 4333", "4123 SomeText 4123-4123 4333")]
		[InlineData("https://www.google.com/search/2330669080123123/", "https://www.google.com/search/2330669080123123/")]
		[InlineData("https://www.google.com/search/2330669080123-123/", "https://www.google.com/search/2330669080123-123/")]
		[Theory]
		public void TestCensorCreditCard(string actual, string expected)
		{
			var sut = new CensorCreditCardsStrategy();
			var message = new Message { Content = actual };
			sut.Censor(message, null);
			Assert.Equal(expected, message.Content);
		}

		[InlineData("34565464", "********")]
		[InlineData("+93483227359", "************")]
		[InlineData("111-123-4567", "************")]
		[InlineData("+934-83227359", "*************")]
		[InlineData("(111)123-4567", "*************")]
		[InlineData("+1703.338.6512", "**************")]
		[InlineData("+1 703 335 65123", "****************")]
		[InlineData("001 (703) 332-6261", "******************")]
		[InlineData("Number: (111)123-4567 so", "Number: ************* so")]
		//Shouldn't be censored.
		[InlineData("1", "1")]
		[InlineData("12", "12")]
		[InlineData("123", "123")]
		[InlineData("1234", "1234")]
		[InlineData("12345", "12345")]
		[InlineData("123456", "123456")]
		[InlineData("https://www.google.com/search/2330669080123123/", "https://www.google.com/search/2330669080123123/")]
		[InlineData("https://www.google.com/search/2330669080123-123/", "https://www.google.com/search/2330669080123-123/")]
		[Theory]
		public void TestCensorPhoneNumber(string actual, string expected)
		{
			var sut = new CensorPhoneNumbersStrategy();
			var message = new Message { Content = actual };
			sut.Censor(message, null);
			Assert.Equal(expected, message.Content);
		}

		//Should censor
		[InlineData("a@m.com", "*******")]
		[InlineData("a@m.co", "******")]
		//Should not censor
		[InlineData("a@ m.co", "a@ m.co")]
		[InlineData("a @m.co", "a @m.co")]
		[InlineData("a @ m.co", "a @ m.co")]
		[Theory]
		public void TestCensorEmailAddress(string actual, string expected)
		{
			var sut = new CensorEmailStrategy();
			var message = new Message { Content = actual };
			sut.Censor(message, null);
			Assert.Equal(expected, message.Content);
		}

		//Social Security Number
		//Should censor
		[InlineData("123-45-6789", "***********")]
		[InlineData("123 45 6789", "***********")]
		[InlineData("123456789", "*********")]
		[InlineData("123-45 6789", "***********")]
		[InlineData("123-456789", "**********")]
		[InlineData("123 456789", "**********")]

		//Should not be censored
		[InlineData("000-45-6789", "000-45-6789")]
		[InlineData("666-45-6789", "666-45-6789")]
		[InlineData("123-00-6789", "123-00-6789")]
		[InlineData("123-45-0000", "123-45-0000")]
		[InlineData("900-45-6789", "900-45-6789")]
		[InlineData("999-45-6789", "999-45-6789")]
		[InlineData("https://www.google.com/search/2330669080123123/", "https://www.google.com/search/2330669080123123/")]
		[InlineData("https://www.google.com/search/2330669080123-123/", "https://www.google.com/search/2330669080123-123/")]
		[Theory]
		public void TestCensorSocialSecurityNumber(string actual, string expected)
		{
			var sut = new CensorSocialSecurityStrategy();
			var message = new Message { Content = actual };
			sut.Censor(message, null);
			Assert.Equal(expected, message.Content);
		}

		[InlineData("AT611904300234573201", "********************")]
		[InlineData("AE070331234567890123456", "***********************")]
		[InlineData("DE91 100000000123456789", "***********************")]
		[InlineData("FR76-30006000011234567890189", "****************************")]
		[Theory]
		public void TestCensorBankAccount(string actual, string expected)
		{
			//Remove irrelevant strategies.
			var sut = new CensorBankAccountStrategy();
			var message = new Message { Content = actual };
			sut.Censor(message, null);
			Assert.Equal(expected, message.Content);
		}

		[InlineData("fake@solvnow.com", "****************")]
		[InlineData("info@solvnow.com", "info@solvnow.com")]
		[InlineData("admin@gov.co.uk", "admin@gov.co.uk")]
		[InlineData("email at end info@solvnow.com", "email at end info@solvnow.com")]
		[InlineData("info@solvnow.com at beginning", "info@solvnow.com at beginning")]
		[InlineData("email occured here info@solvnow.com and here admin@gov.co.uk", "email occured here info@solvnow.com and here admin@gov.co.uk")]
		[InlineData("email to censor here info@solvnow.com and not to censor here fake@gov.co.uk", "email to censor here info@solvnow.com and not to censor here **************")]
		[Theory]
		public void ShouldNotCensorWhitelistedPhrases(string actual, string expected)
		{
			var whitelist = new List<string>
			{
				"info@solvnow.com",
				"admin@gov.co.uk"
			};

			var sut = new CensorEmailStrategy();
			var message = new Message { Content = actual };
			sut.Censor(message, whitelist);
			Assert.Equal(expected, message.Content);
		}
	}
}