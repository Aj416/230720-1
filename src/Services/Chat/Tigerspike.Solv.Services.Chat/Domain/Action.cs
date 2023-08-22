using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Services.Chat.Domain;

namespace Tigerspike.Solv.Chat.Domain
{
	public class Action : IRecord
	{
		private Guid _id;

		[HashKey]
		public string HashKey { get; private set; }

		[RangeKey]
		public string RangeKey { get; private set; }

		public string RecordType { get; private set; }

		/// <summary>
		/// The id of the action.
		/// </summary>
		public Guid Id
		{
			get => _id;
			set
			{
				_id = value;
				HashKey = $"{nameof(Action)}#{_id}";
				RangeKey = $"{nameof(Action)}#{_id}";
			}
		}

		/// <summary>
		/// Type of action (question, feedback requests etc.)
		/// </summary>
		[Required]
		public int Type { get; set; }

		/// <summary>
		/// Has action been finalized
		/// </summary>
		public bool IsFinalized { get; set; }

		/// <summary>
		/// Is action blocking chat until acted upon
		/// </summary>
		public bool IsBlocking { get; set; }

		/// <summary>
		/// Options available in action
		/// </summary>
		public ICollection<ActionOption> Options { get; set; }

		protected Action()
		{
			RecordType = $"{nameof(Action)}";
		}

		public Action(Guid id, int type, bool isBlocking, List<ActionOption> options)
		{
			Id = id;
			Type = type;
			IsBlocking = isBlocking;
			Options = options;

			RecordType = $"{nameof(Action)}";
		}
	}
}