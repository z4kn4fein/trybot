using System;

namespace Trybot.CircuitBreaker
{
    /// <summary>
    /// This service used to indicate a circuit state switch by the <see cref="ICircuitBreakerStrategy"/>.
    /// </summary>
    public interface ICircuitBreakerStateSwitcher
    {
        /// <summary>
        /// Attempts to close the circuit.
        /// </summary>
        void Close();

        /// <summary>
        /// Attempts to open the circuit.
        /// </summary>
        void Open(TimeSpan duration);
    }
}
