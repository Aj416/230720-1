namespace Tigerspike.Solv.Messaging.Chat
{
    public interface IChatActionOptionSideEffect
    {
        /// <summary>
		/// Type of side effect
		/// </summary>
        public int Effect { get; set; }

        /// <summary>
        /// Type of side effect
        /// </summary>
        public string Value { get; set; }
    }
}