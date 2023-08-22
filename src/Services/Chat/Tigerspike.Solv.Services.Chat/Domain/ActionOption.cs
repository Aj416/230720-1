using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;

namespace Tigerspike.Solv.Chat.Domain
{
	public class ActionOption
	{
		private Guid _id;
		private Guid _chatActionId;

		[HashKey]
		public string HashKey { get; private set; }

		[RangeKey]
		public string RangeKey { get; private set; }

		public string RecordType { get; private set; }

		/// <summary>
		/// The action option id.
		/// </summary>
		public Guid Id
		{
			get => _id;
			set
			{
				_id = value;
				RangeKey = $"{nameof(ActionOption)}#{_id}";
			}
		}

		/// <summary>
		/// The action id.
		/// </summary>
		public Guid ChatActionId
		{
			get => _chatActionId;
			set
			{
				_chatActionId = value;
				HashKey = $"{nameof(Action)}#{_chatActionId}";
			}
		}

		/// <summary>
		/// Label of the option
		/// </summary>
		public string Label { get; set; }

		/// <summary>
		/// Value of the option
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Is option selected (chosen)
		/// </summary>
		public bool IsSelected { get; set; }

		/// <summary>
		/// Whether it is suggested option to chose
		/// </summary>
		public bool IsSuggested { get; set; }

		/// <summary>
		/// Side effects for action
		/// </summary>
		public ICollection<SideEffect> SideEffects { get; set; }

		protected ActionOption()
		{
			RecordType = $"{nameof(ActionOption)}";
		}

		public ActionOption(Guid id, Guid chatActionId, string label, string value, bool isSuggested,
			List<SideEffect> sideEffects)
		{
			Id = id;
			ChatActionId = chatActionId;
			Label = label;
			Value = value;
			IsSuggested = isSuggested;
			SideEffects = sideEffects;

			RecordType = $"{nameof(ActionOption)}";
		}
	}
}