using System;
using AutoMapper;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Messaging.Invoicing;
using Tigerspike.Solv.Services.Invoicing.Domain;
using Tigerspike.Solv.Services.Invoicing.Models;

namespace Tigerspike.Solv.Services.Invoicing.Application.AutoMapper
{
	public class InvoiceProfile : Profile
	{
		public InvoiceProfile()
		{
			CreateMap<AdvocateInvoice, AdvocateInvoiceModel>()
			.ForMember(x => x.AdvocateStatus, x => x.Ignore())
			.ForMember(x => x.AdvocateFullName, x => x.Ignore());

			CreateMap<ClientInvoice, ClientInvoiceModel>();

			CreateMap<ClientInvoice, ClientInvoicePrintableModel>()
				.ForMember(x => x.PaymentRouteName, x => x.Ignore());

			CreateMap<InvoicingCycle, InvoicingCyclePrintableModel>()
				.ForMember(x => x.To, x => x.MapFrom(src => src.To.AddDays(-1).Date)); // we want to the last day of the cycle as the date on invoice, not the right boundary

			CreateMap<BillingDetails, BillingDetailsPrintableModel>()
				.ForMember(x => x.AddressLines, x =>
				{
					x.PreCondition(src => src.Address != null);
					x.MapFrom(src => src.Address.SplitAndTrim(Environment.NewLine));
				});

			CreateMap<ITicketInfoResult, TicketPrintableModel>();

			CreateMap<AdvocateInvoice, AdvocateInvoicePrintableModel>();

			CreateMap<AdvocateInvoiceLineItem, AdvocateInvoiceLineItemPrintableModel>()
				.ForMember(x => x.BrandId, x => x.MapFrom(src => src.BrandId))
				.ForMember(x => x.Brand, x => x.MapFrom(src => src.BrandName));
		}
	}
}
