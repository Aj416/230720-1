namespace Tigerspike.Solv.Application.Constants
{
	public static class SystemMessage
	{
		public const string TicketClosed = "This is the end of the support. Thank you, enjoy your day";
		public const string TicketSolved = "The ticket has been marked as solved";
		public const string TicketReopened = "The ticket has not been solved";
		public static string[] PractiseTicketMessages => new string[]{
			"This is a practice ticket, which means there is no real customer on the other side",
			"Please follow these steps to finish the practice mode:",
			"1- Once you've pulled the ticket add at least one reply to this conversation.\n" +
			"2- Fill in the details in the right hand panel.\n" +
			"3- Select the Category type from the dropdown.\n" +
			"4- Select the appropriate tags.\n" +
			"5- Answer whether there's a sales lead.\n" +
			"6- Close the ticket by clicking 'Mark as solved'.\n" +
			"7- Rate the complexity by moving the slider below when prompted.\n" +
			"Once you finish the last step, you will be redirected to the profile page where you can authorise for a brand.",
			"Happy Solving from Solv team"
		};
	}
}