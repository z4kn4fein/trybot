using System;

namespace Trybot.CircuitBreaker
{
    /// <summary>
    /// Contains shared members for the circuit breaker configurations.
    /// </summary>
    public class CircuitBreakerConfigurationBase
    {
        internal Func<Exception, bool> ExceptionPolicy { get; set; }

        internal Action<TimeSpan> OpenStateHandler { get; set; }

        internal TimeSpan OpenStateDuration { get; set; }

        internal Action HalfOpenStateHandler { get; set; }

        internal Action ClosedStateHandler { get; set; }

        internal ICircuitStateHandler StateHandler { get; set; } = new InMemoryCircuitStateStore();

        internal bool HandlesException(Exception exception) =>
            this.ExceptionPolicy?.Invoke(exception) ?? false;
    }

    /// <summary>
    /// Represents the configuration of the circuit breaker bot.
    /// </summary>
    public class CircuitBreakerConfiguration : CircuitBreakerConfigurationBase, ICircuitBreakerConfiguration<CircuitBreakerConfiguration>
    {
        /// <inheritdoc />
        public CircuitBreakerConfiguration BrakeWhenExceptionOccurs(Func<Exception, bool> exceptionPolicy)
        {
            base.ExceptionPolicy = exceptionPolicy;
            return this;
        }

        /// <inheritdoc />
        public CircuitBreakerConfiguration WithStateHandler(ICircuitStateHandler stateHandler)
        {
            base.StateHandler = stateHandler;
            return this;
        }

        /// <inheritdoc />
        public CircuitBreakerConfiguration OnOpen(Action<TimeSpan> openHandler)
        {
            base.OpenStateHandler = openHandler;
            return this;
        }

        /// <inheritdoc />
        public CircuitBreakerConfiguration OnClosed(Action closedHandler)
        {
            base.ClosedStateHandler = closedHandler;
            return this;
        }

        /// <inheritdoc />
        public CircuitBreakerConfiguration OnHalfOpen(Action halfOpenHandler)
        {
            base.HalfOpenStateHandler = halfOpenHandler;
            return this;
        }

        /// <inheritdoc />
        public CircuitBreakerConfiguration DurationOfOpen(TimeSpan openStateDuration)
        {
            this.OpenStateDuration = openStateDuration;
            return this;
        }
    }

    /// <summary>
    /// Represents the configuration of the circuit breaker bot.
    /// </summary>
    public class CircuitBreakerConfiguration<TResult> : CircuitBreakerConfigurationBase, ICircuitBreakerConfiguration<CircuitBreakerConfiguration<TResult>, TResult>
    {
        private Func<TResult, bool> resultPolicy;

        /// <inheritdoc />
        public CircuitBreakerConfiguration<TResult> BrakeWhenExceptionOccurs(Func<Exception, bool> exceptionPolicy)
        {
            base.ExceptionPolicy = exceptionPolicy;
            return this;
        }

        /// <inheritdoc />
        public CircuitBreakerConfiguration<TResult> BrakeWhenResultIs(Func<TResult, bool> resultPolicy)
        {
            this.resultPolicy = resultPolicy;
            return this;
        }

        /// <inheritdoc />
        public CircuitBreakerConfiguration<TResult> WithStateHandler(ICircuitStateHandler stateHandler)
        {
            base.StateHandler = stateHandler;
            return this;
        }

        /// <inheritdoc />
        public CircuitBreakerConfiguration<TResult> OnOpen(Action<TimeSpan> openHandler)
        {
            base.OpenStateHandler = openHandler;
            return this;
        }

        /// <inheritdoc />
        public CircuitBreakerConfiguration<TResult> OnClosed(Action closedHandler)
        {
            base.ClosedStateHandler = closedHandler;
            return this;
        }

        /// <inheritdoc />
        public CircuitBreakerConfiguration<TResult> OnHalfOpen(Action halfOpenHandler)
        {
            base.HalfOpenStateHandler = halfOpenHandler;
            return this;
        }

        /// <inheritdoc />
        public CircuitBreakerConfiguration<TResult> DurationOfOpen(TimeSpan openStateDuration)
        {
            this.OpenStateDuration = openStateDuration;
            return this;
        }

        internal bool AcceptsResult(TResult result) =>
            !this.resultPolicy?.Invoke(result) ?? true;

    }
}