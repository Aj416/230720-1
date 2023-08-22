using System;
using System.Collections.Generic;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Application.Commands.Fraud
{
	/// <inheritdoc />
	public class DetectFraudCommand : IDetectFraudCommand
	{
		/// <inheritdoc />
		public IList<ISerialNumber> SerialNumberInfoForLastWeek { get; set; } = new List<ISerialNumber>();

		/// <inheritdoc />
		public IList<ICustomer> CustomerInfoForLastDay { get; set; } = new List<ICustomer>();

		/// <inheritdoc />
		public Guid BrandId { get; set; }

		/// <inheritdoc />
		public string BrandName { get; set; }

		/// <inheritdoc />
		public string SolverIp { get; set; }

		/// <inheritdoc />
		public string CustomerIp { get; set; }

		/// <inheritdoc />
		public int? CloseTime { get; set; }

		/// <inheritdoc />
		public int? Csat { get; set; }

		/// <inheritdoc />
		public int TicketStatus { get; set; }

		/// <inheritdoc />
		public int Level { get; set; }

		/// <inheritdoc />
		public DateTime CreatedDate { get; set; }

		/// <inheritdoc />
		public Guid TicketId { get; set; }

		/// <inheritdoc />
		public string AdvocateName { get; set; }

		/// <inheritdoc />
		public Guid? CustomerId { get; set; }

		/// <inheritdoc />
		public bool? ClosedByCustomer { get; set; }

		/// <inheritdoc />
		public IList<int> ResponseTimes { get; set; } = new List<int>();

		/// <inheritdoc />
		public IList<ISerialNumber> SerialNumberInfoForLastThreeDays { get; set; } = new List<ISerialNumber>();

		/// <inheritdoc />
		public int? CurrentResponseTimeInMinutes { get; set; }

		/// <inheritdoc />
		public IDictionary<string, string> Metadata { get; set; }

		/// <inheritdoc />
		public string CustomerFirstName { get; set; }

		/// <inheritdoc />
		public string CustomerLastName { get; set; }

		/// <inheritdoc />
		public string CustomerEmail { get; set; }

		/// <inheritdoc />
		public string Question { get; set; }

		/// <summary>
		/// Initialize DetectFraudCommand in case of TicketCreated Event.
		/// </summary>
		public DetectFraudCommand(List<ISerialNumber> serialNumberInfoForLastWeek, List<ISerialNumber> serialNumberInfoForLastThreeDays, List<ICustomer> customerInfoForLastDay,
				int ticketStatus, int level, DateTime createdDate, Guid ticketId, Guid brandId, string brandName, Guid customerId, string customerIp, string solverIp,
				IDictionary<string, string> metadata, string customerFirstName, string customerLastName, string customerEmail, string question)
		{
			SerialNumberInfoForLastWeek = serialNumberInfoForLastWeek;
			SerialNumberInfoForLastThreeDays = serialNumberInfoForLastThreeDays;
			CustomerInfoForLastDay = customerInfoForLastDay;
			TicketStatus = ticketStatus;
			Level = level;
			CreatedDate = createdDate;
			TicketId = ticketId;
			BrandId = brandId;
			CustomerId = customerId;
			CustomerIp = customerIp;
			SolverIp = solverIp;
			Metadata = metadata;
			CustomerFirstName = customerFirstName;
			CustomerLastName = customerLastName;
			CustomerEmail = customerEmail;
			BrandName = brandName;
			Question = question;
		}

		/// <summary>
		/// Initialize DetectFraudCommand in case of TicketAccepted Event.
		/// </summary>
		public DetectFraudCommand(List<ISerialNumber> serialNumberInfoForLastWeek,
			int ticketStatus, int level, Guid ticketId, string advocateName, Guid brandId, string brandName, Guid customerId, string customerIp, string solverIp)
		{
			SerialNumberInfoForLastWeek = serialNumberInfoForLastWeek;
			TicketStatus = ticketStatus;
			Level = level;
			TicketId = ticketId;
			AdvocateName = advocateName;
			BrandId = brandId;
			CustomerId = customerId;
			CustomerIp = customerIp;
			SolverIp = solverIp;
			BrandName = brandName;
		}

		/// <summary>
		/// Initialize DetectFraudCommand in case of TicketSolved Event.
		/// </summary>
		public DetectFraudCommand(int ticketStatus, int level, Guid ticketId, string advocateName, Guid brandId, string brandName,
			Guid customerId)
		{
			TicketStatus = ticketStatus;
			Level = level;
			TicketId = ticketId;
			AdvocateName = advocateName;
			BrandId = brandId;
			CustomerId = customerId;
			BrandName = brandName;
		}

		/// <summary>
		/// Initialize DetectFraudCommand in case of TicketClosed Event.
		/// </summary>
		public DetectFraudCommand(int? currentResponseTimeInMinutes, List<int> responseTimes, int? closeTime, Guid ticketId, int? csat, bool closedByCustomer, int ticketStatus, int level, string advocateName, Guid brandId, string brandName,
			Guid customerId)
		{
			CurrentResponseTimeInMinutes = currentResponseTimeInMinutes;
			ResponseTimes = responseTimes;
			CloseTime = closeTime;
			Csat = csat;
			ClosedByCustomer = closedByCustomer;
			TicketStatus = ticketStatus;
			Level = level;
			TicketId = ticketId;
			AdvocateName = advocateName;
			BrandId = brandId;
			CustomerId = customerId;
			BrandName = brandName;
		}

	}
}