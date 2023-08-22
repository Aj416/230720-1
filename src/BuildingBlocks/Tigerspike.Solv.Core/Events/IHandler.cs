﻿namespace Tigerspike.Solv.Core.Events
{
	public interface IHandler<in T> where T : Message
	{
		void Handle(T message);
	}
}