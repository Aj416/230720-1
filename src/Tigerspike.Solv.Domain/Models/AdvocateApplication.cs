using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Interfaces;
using Tigerspike.Solv.Domain.Models.Profile;
using Microsoft.AspNetCore.WebUtilities;

namespace Tigerspike.Solv.Domain.Models
{
	public class AdvocateApplication : ICreatedDate
	{
		public AdvocateApplication(string firstName, string lastName, string email, string phone, string state,
			string country, string source, bool internalAgent, string address, string city, string zipCode, Guid? id = null)
		{
			Id = id ?? Guid.NewGuid();
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			Phone = phone;
			State = state;
			Country = country;
			ApplicationStatus = AdvocateApplicationStatus.New;
			Source = source;
			InternalAgent = internalAgent;
			CompletedEmailSent = internalAgent; // if application is for internal agent, then we skip this step and mark it as done
			Token = GenerateToken();
			Address = address;
			City = city;
			ZipCode = zipCode;
		}

		/// <summary>
		/// Private constructor for EF only
		/// </summary>
		private AdvocateApplication() { }

		/// <summary>
		/// Unique GUID primary key
		/// </summary>
		public Guid Id { get; private set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The country of the advocate.
		/// </summary>
		public string Country { get; private set; }

		/// <summary>
		/// The state of the advocate.
		/// </summary>
		public string State { get; private set; }

		/// <summary>
		/// The Address of the advocate.
		/// </summary>
		public string Address { get; private set; }

		/// <summary>
		/// The City of the advocate.
		/// </summary>
		public string City { get; private set; }

		/// <summary>
		/// The Zip Code of the advocate.
		/// </summary>
		public string ZipCode { get; private set; }

		/// <summary>
		/// The email address of the advocate.
		/// </summary>
		public string Email { get; private set; }

		/// <summary>
		/// User's first name
		/// </summary>
		public string FirstName { get; private set; }

		/// <summary>
		/// User's last name
		/// </summary>
		public string LastName { get; private set; }

		/// <summary>
		/// User's phone number
		/// </summary>
		public string Phone { get; private set; }

		/// <summary>
		/// Where the applicant heard of Solv.
		/// </summary>
		public string Source { get; private set; }

		/// <summary>
		/// Whether the thank you email has been sent.
		/// </summary>
		public bool ResponseEmailSent { get; private set; }

		/// <summary>
		/// Whether the response email has been sent after the answers have been submitted.
		/// </summary>
		public bool CompletedEmailSent { get; private set; }

		/// <summary>
		/// Whether or not an invitation to the platform has been sent
		/// </summary>
		public bool InvitationEmailSent { get; private set; }

		/// <summary>
		/// The randomly generated GUID that the advocate can use to delete their data.
		/// </summary>
		public string DeletionHash { get; set; }

		/// <summary>
		/// The randomly generated token that the advocate can use to answer profiling or signup.
		/// </summary>
		public string Token { get; private set; }

		/// <summary>
		/// The current status of the application
		/// </summary>
		public AdvocateApplicationStatus ApplicationStatus { get; set; }

		/// <summary>
		/// Date an invitation was sent
		/// </summary>
		public DateTime? InvitationDate { get; set; }

		/// <summary>
		/// Last Date of invitation was sent
		/// </summary>
		public DateTime? LastInvitationDate { get; set; }

		/// <summary>
		/// Whether application is for internal agent
		/// </summary>
		public bool InternalAgent { get; set; }

		/// <summary>
		/// Answers given during the Application process
		/// </summary>
		public ICollection<ApplicationAnswer> ApplicationAnswers { get; set; }

		/// <summary>
		/// Answers given during the Application process
		/// </summary>
		public ICollection<AdvocateApplicationBrand> BrandAssignments { get; set; }

		/// <summary>
		/// Invites the advocate to join the platform by sending them an email, marking their
		/// application as invited and also setting the InvitationDate to the current UTC date/time.
		/// </summary>
		public void Invite()
		{
			InvitationEmailSent = true;
			ApplicationStatus = AdvocateApplicationStatus.Invited;
			InvitationDate = DateTime.UtcNow;
		}

		/// <summary>
		/// Declines the advocate to join the platform by marking their application as not suitable.
		/// </summary>
		public void Decline()
		{
			InvitationEmailSent = false;
			ApplicationStatus = AdvocateApplicationStatus.NotSuitable;
		}

		public void SetResponseEmailSent() => ResponseEmailSent = true;
		public void SetCompletedEmailSent() => CompletedEmailSent = true;
		public void SetPhone(string phone) => Phone = phone;

		/// <summary>
		/// Finishes the application by invalidating the token so it can no longer be used and set the ApplicationStatus to Created. Used when the advocate has created an account based on this application
		/// </summary>
		/// <returns>The token that was generated</returns>
		public void Finish()
		{
			Token = null;
			ApplicationStatus = AdvocateApplicationStatus.AccountCreated;
		}

		public void SetAnswers(List<ApplicationAnswer> answers)
		{
			ApplicationAnswers = new List<ApplicationAnswer>();
			foreach (var answer in answers)
			{
				ApplicationAnswers.Add(answer);
			}
		}

		/// <summary>
		/// Generates a randomly crypted token mixed with the id of the application for uniquness
		/// </summary>
		/// <returns>The token that was generated</returns>
		private string GenerateToken()
		{
			// Generate a cryptographic random number.
			var rng = new RNGCryptoServiceProvider();
			var buff = new byte[256];
			rng.GetBytes(buff);
			var salt = Convert.ToBase64String(buff);

			var bytes = Encoding.UTF8.GetBytes(Id.ToString() + salt);
			var sHA256ManagedString = new SHA256Managed();
			var hash = sHA256ManagedString.ComputeHash(bytes);
			return WebEncoders.Base64UrlEncode(hash);
		}

		/// <summary>
		/// Update the name of advocate.
		/// </summary>
		/// <param name="firstName">The first name of advocate.</param>
		/// <param name="lastName">The last name of advocate.</param>
		public void ChangeName(string firstName, string lastName)
		{
			FirstName = firstName;
			LastName = lastName;
		}
	}
}