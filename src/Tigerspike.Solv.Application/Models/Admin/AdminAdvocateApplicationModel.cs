using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Admin
{
	public class AdminAdvocateApplicationModel
	{
		public AdvocateApplicationModel Application { get; set; }

		public DateTime? AccountCreatedDate { get; set; }

		public List<string> Brands { get; set; }

		public List<AdminAreaModel> Areas { get; set; } = new List<AdminAreaModel>();
	}
}