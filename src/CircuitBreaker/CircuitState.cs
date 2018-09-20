namespace Trybot.CircuitBreaker
{
    /// <summary>
    /// Represents a circuit state.
    /// </summary>
    public class CircuitState
    {
        /// <summary>
        /// The closed state.
        /// </summary>
        public static readonly CircuitState Closed = new CircuitState();

        /// <summary>
        /// The half open state.
        /// </summary>
        public static readonly CircuitState HalfOpen = new CircuitState();

        /// <summary>
        /// The open state.
        /// </summary>
        public static readonly CircuitState Open = new CircuitState();

        private CircuitState() { }
    }
}
