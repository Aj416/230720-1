using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Services.Invoicing.Application.Commands
{
	public class CreateInvoicingCycleCommand : Command<Guid?>
	{
		/// <summary>
		/// Gets or sets InvoicingCycleStartDate
		/// </summary>
		public DateTime InvoicingCycleStartDate { get; set; }

		/// <summary>
		/// Gets or sets InvoicingCycleEndDate
		/// </summary>
		public DateTime InvoicingCycleEndDate { get; set; }


		public CreateInvoicingCycleCommand(DateTime invoicingCycleStartDate, DateTime invoicingCycleEndDate)
		{
			InvoicingCycleStartDate = invoicingCycleStartDate;
			InvoicingCycleEndDate = invoicingCycleEndDate;
		}

		public override bool IsValid() => InvoicingCycleStartDate > DateTime.MinValue && InvoicingCycleEndDate > DateTime.MinValue;
	}
}
