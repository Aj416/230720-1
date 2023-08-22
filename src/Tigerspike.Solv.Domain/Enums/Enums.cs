namespace Tigerspike.Solv.Domain.Enums
{
	public enum AdvocateApplicationStatus
	{
		New = 1,
		Invited = 2,
		NotSuitable = 3,
		AccountCreated = 4
	}

	public enum AdminAdvocateApplicationStatusSortBy
	{
		Unspecified = 0,
		FirstName = 1,
		LastName = 2,
		Country = 3,
		CreatedDate = 4,
		Source = 5,
		CompletedEmailSent = 6,
		InvitationDate = 7,
		FullName = 8,
		Language = 9,
		Skills = 10,
	}

	/// <summary>
	/// The list of available sort columns
	/// </summary>
	public enum TicketImportSortBy
	{
		unspecified,
		id,
		uploadDate,
		status,
		user,
		ClosedDate,
		price,
		tags,
		total,
		imported,
		failed,
	}
}