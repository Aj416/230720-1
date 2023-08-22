using System;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Commands.Validations;

namespace Tigerspike.Solv.Domain.Commands
{
	public class CreateAdvocateApplicationCommand : Command<Guid?>
	{
		public CreateAdvocateApplicationCommand(string country, string state, string email, string firstName, string lastName, string phone, string source, string isAdult, string marketingCheckbox, bool internalAgent, string address, string city, string zipCode, string dataPolicyCheckBox, string password)
		{
			Country = country;
			State = state;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			Phone = phone;
			Source = source;
			IsAdult = isAdult;
			MarketingCheckbox = marketingCheckbox;
			InternalAgent = internalAgent;
			Address = address;
			City = city;
			ZipCode = zipCode;
			DataPolicyCheckbox = dataPolicyCheckBox;
			Password = password;
		}

		/// <summary>
		/// The country of the advocate.
		/// </summary>
		public string Country { get; set; }

		/// <summary>
		/// The state of the advocate.
		/// </summary>
		public string State { get; set; }

		/// <summary>
		/// The email address of the advocate.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// First name of the advocate.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Last name of the advocate.
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// Advocate's phone number.
		/// </summary>
		public string Phone { get; set; }

		/// <summary>
		/// Source where the applicant heard of Solv.
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Whether or not the advocate is old enough to use this platform.
		/// </summary>
		public string IsAdult { get; set; }

		/// <summary>
		/// Whether or not the advocate consents to marketing.
		/// </summary>
		public string MarketingCheckbox { get; set; }

		public bool InternalAgent { get; set; }

		/// <summary>
		/// The Address of the advocate.
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// The City of the advocate.
		/// </summary>
		public string City { get; set; }

		/// <summary>
		/// The Zip Code of the advocate.
		/// </summary>
		public string ZipCode { get; set; }

		/// <summary>
		/// Whether or not the advocate consents to Data Policy.
		/// </summary>
		public string DataPolicyCheckbox { get; set; }

		/// <summary>
		/// Advocate password
		/// </summary>
		public string Password { get; set; }

		public override bool IsValid()
		{
			ValidationResult = new CreateAdvocateApplicationCommandValidator().Validate(this);
			return ValidationResult.IsValid;
		}
	}
}