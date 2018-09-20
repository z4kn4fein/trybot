namespace Trybot.CircuitBreaker
{
    /// <summary>
    /// Represents a circuit breaker strategy used by the circuit bot internally.
    /// It defines the actual behavior of the circuit breaker based on the given operation related events.
    /// </summary>
    public interface ICircuitBreakerStrategy
    {
        /// <summary>
        /// Called when the underlying operation is failed within the Closed circuit state.
        /// </summary>
        void OperationFailedInClosed();

        /// <summary>
        /// Called when the underlying operation is succeeded within the Closed circuit state.
        /// </summary>
        void OperationSucceededInClosed();

        /// <summary>
        /// Called when the underlying operation is failed within the HalfOpen circuit state.
        /// </summary>
        void OperationFailedInHalfOpen();

        /// <summary>
        /// Called when the underlying operation is succeeded within the HalfOpen circuit state.
        /// </summary>
        void OperationSucceededInHalfOpen();
    }
}
