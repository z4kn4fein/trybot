namespace Trybot.CircuitBreaker
{
    /// <summary>
    /// Represents an abstract circuit breaker strategy.
    /// </summary>
    public abstract class CircuitBreakerStrategy : ICircuitBreakerStrategy
    {
        /// <summary>
        /// The circuit breaker state switcher.
        /// </summary>
        protected ICircuitBreakerStateSwitcher Switcher { get; }

        /// <summary>
        /// Constructs a <see cref="CircuitBreakerStrategy" /> implementation.
        /// </summary>
        /// <param name="switcher">The circuit breaker state switcher.</param>
        protected CircuitBreakerStrategy(ICircuitBreakerStateSwitcher switcher)
        {
            this.Switcher = switcher;
        }

        /// <inheritdoc />
        public abstract void OperationFailedInClosed();

        /// <inheritdoc />
        public abstract void OperationFailedInHalfOpen();

        /// <inheritdoc />
        public abstract void OperationSucceededInClosed();

        /// <inheritdoc />
        public abstract void OperationSucceededInHalfOpen();
    }
}
