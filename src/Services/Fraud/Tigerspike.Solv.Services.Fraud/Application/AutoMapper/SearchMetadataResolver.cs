using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Tigerspike.Solv.Services.Fraud.Models;

namespace Tigerspike.Solv.Services.Fraud.Application.AutoMapper
{
	public class SearchMetadataResolver : IValueResolver<TicketModel, FraudSearchModel, IDictionary<string, string>>
	{
		public IDictionary<string, string> Resolve(TicketModel source, FraudSearchModel destination, IDictionary<string, string> destMember, ResolutionContext context)
        {
            IDictionary<string,string> values = new Dictionary<string, string>();

            if(source.Metadata != null && source.Metadata.Any())
            {
                foreach(KeyValuePair<string,string> pair in source.Metadata)
                {
                    values.Add(pair.Key, pair.Value);
                }
            }
            
            if(source.CustomerDetail != null) 
            {
                values.Add("customerName", source.CustomerDetail.FullName);
                values.Add("customerEmail", source.CustomerDetail.Email);
            }

            return context.Mapper.Map<IDictionary<string,string>>(values);
        }
	}
}