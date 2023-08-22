using System;
using System.Linq;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Application.Services
{
	public class AssignBrandsToAdvocateApplicationsCommand : Command<Unit>
	{
		/// <summary>
		/// The brand Id
		/// </summary>
		public Guid[] BrandIds { get; set; }

		/// <summary>
		/// The list of advocate applications to be assigned the brand.
		/// </summary>
		public Guid[] AdvocateApplicationIds { get; set; }

		public AssignBrandsToAdvocateApplicationsCommand(Guid[] brandIds, Guid[] advocateApplicationIds)
		{
			BrandIds = brandIds;
			AdvocateApplicationIds = advocateApplicationIds;
		}

		public override bool IsValid() => BrandIds.Any() && AdvocateApplicationIds.Any();
	}
}