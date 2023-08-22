using System;
using System.Linq;
using AutoMapper;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Application.AutoMapper
{
	public class InvoiceProfile : Profile
	{
		public InvoiceProfile()
		{
			CreateMap<BillingDetails, BillingDetailsPrintableModel>()
				.ForMember(x => x.AddressLines, x =>
				{
					x.PreCondition(src => src.Address != null);
					x.MapFrom(src => src.Address.SplitAndTrim(Environment.NewLine));
				});

			CreateMap<ClientInvoice, ClientInvoiceModel>();
			CreateMap<ClientInvoice, ClientInvoicePrintableModel>()
				.ForMember(x => x.PaymentRouteName, x => x.MapFrom(src => src.Brand.PaymentRoute != null ? src.Brand.PaymentRoute.Name : null))
				.ForMember(x => x.Tickets, x => x.MapFrom(src => src.Tickets.OrderBy(o => o.CreatedDate)));

			CreateMap<AdvocateInvoice, AdvocateInvoiceModel>()
				.ForMember(x => x.AdvocateFullName, x => x.MapFrom(src =>
					src.Advocate != null &&
					src.Advocate.User != null ? $"{src.Advocate.User.FirstName} {src.Advocate.User.LastName}" : null))
				.ForMember(x => x.AdvocateStatus, x => x.MapFrom(src =>
					src.Advocate != null ? src.Advocate.Status : Domain.Enums.AdvocateStatus.Unknown));

			CreateMap<AdvocateInvoice, AdvocateInvoicePrintableModel>();
			CreateMap<AdvocateInvoiceLineItem, AdvocateInvoiceLineItemPrintableModel>()
				.ForMember(x => x.Brand, x => x.MapFrom(src => src.Brand.Name));

			CreateMap<InvoicingCycle, InvoicingCyclePrintableModel>()
				.ForMember(x => x.To, x => x.MapFrom(src => src.To.AddDays(-1).Date)); // we want to the last day of the cycle as the date on invoice, not the right boundary
		}
	}
}