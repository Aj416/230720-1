using System.Collections.Generic;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Commands.Validations;
using MediatR;

namespace Tigerspike.Solv.Domain.Commands
{
	public class UpdateSuperSolverCommand : Command<Unit>
	{
		public UpdateSuperSolverCommand(List<AdvocateApplication> advocateApplications)
		{
			AdvocateApplications = advocateApplications;
		}

		public List<AdvocateApplication> AdvocateApplications { get; set; }

		public override bool IsValid() => AdvocateApplications.Count > 0;
	}
}