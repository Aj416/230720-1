using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models
{
	public class ProfileSkillModel
	{
		public string Name { get; set; }
		public string RawLevel { get; set; }
		private ProfileSkillLevel Level => Enum.TryParse<ProfileSkillLevel>(RawLevel, out var result) ? result : ProfileSkillLevel.None;
		public string Display => Level != ProfileSkillLevel.None ? $"{Name} ({Level})" : Name;
	}
}