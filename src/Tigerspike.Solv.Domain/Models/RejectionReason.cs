namespace Tigerspike.Solv.Domain.Models
{
	public class RejectionReason
	{
		public const int ReservationExpiredReasonId = 408; // as in http :)
		public const string ReservationExpiredReasonName = "Reservation has expired";

		public int Id { get; set; }

		public string Name { get; set; }
	}
}
