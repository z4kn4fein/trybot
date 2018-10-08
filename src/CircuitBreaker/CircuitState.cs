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
        public static readonly CircuitState Closed = new CircuitState("Closed");

        /// <summary>
        /// The half open state.
        /// </summary>
        public static readonly CircuitState HalfOpen = new CircuitState("HalfOpen");

        /// <summary>
        /// The open state.
        /// </summary>
        public static readonly CircuitState Open = new CircuitState("Open");

        /// <summary>
        /// The name of the state.
        /// </summary>
        public string Name { get; }

        private CircuitState(string name)
        {
            this.Name = name;
        }
    }
}
