namespace Trybot.CircuitBreaker
{
    /// <summary>
    /// Represents a store which will be used to save and load the circuit state.
    /// </summary>
    public interface ICircuitStateStore
    {
        /// <summary>
        /// Loads the circuit state.
        /// </summary>
        /// <returns>The state.</returns>
        CircuitState Get();

        /// <summary>
        /// Saves the circuit state.
        /// </summary>
        /// <param name="state">The state.</param>
        void Set(CircuitState state);
    }
}
