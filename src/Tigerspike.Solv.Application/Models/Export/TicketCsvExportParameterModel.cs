using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Export
{
    public class TicketCsvExportParameterModel
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? BrandId { get; set; }
        public CsvExportSource TriggeredBy { get; set; }
    }
}