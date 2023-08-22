using System;
using Tigerspike.Solv.Chat.Enums;

namespace Tigerspike.Solv.Chat.Domain
{
	public class SideEffect
	{
		public Guid Id { get; set; }

		public SideEffectType Effect { get; set; }

		public string Value { get; set; }

		protected SideEffect() {}

		public SideEffect(Guid id, SideEffectType effect, string value)
		{
			Id = id;
			Effect = effect;
			Value = value;
		}
	}
}