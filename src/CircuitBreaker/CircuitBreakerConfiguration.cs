using System;

namespace Trybot.CircuitBreaker
{
    public class CircuitBreakerConfigurationBase
    {
        protected Func<Exception, bool> ExceptionPolicy { get; set; }

        internal Action<TimeSpan> OpenStateHandler { get; set; }

        internal Action HalfOpenStateHandler { get; set; }

        internal Action ClosedStateHandler { get; set; }

        internal ICircuitStateStore StateStore { get; set; } = new InMemoryCircuitStateStore();

        internal bool HandlesException(Exception exception) =>
            this.ExceptionPolicy?.Invoke(exception) ?? false;
    }

    public class CircuitBreakerConfiguration : CircuitBreakerConfigurationBase, ICircuitBreakerConfiguration<CircuitBreakerConfiguration>
    {
        public CircuitBreakerConfiguration BrakeWhenExceptionOccurs(Func<Exception, bool> exceptionPolicy)
        {
            base.ExceptionPolicy = exceptionPolicy;
            return this;
        }

        public CircuitBreakerConfiguration WithStateStore(ICircuitStateStore stateStore)
        {
            base.StateStore = stateStore;
            return this;
        }

        public CircuitBreakerConfiguration OnOpen(Action<TimeSpan> openHandler)
        {
            base.OpenStateHandler = openHandler;
            return this;
        }

        public CircuitBreakerConfiguration OnClosed(Action closedHandler)
        {
            base.ClosedStateHandler = closedHandler;
            return this;
        }

        public CircuitBreakerConfiguration OnHalfOpen(Action halfOpenHandler)
        {
            base.HalfOpenStateHandler = halfOpenHandler;
            return this;
        }
    }

    public class CircuitBreakerConfiguration<TResult> : CircuitBreakerConfigurationBase, ICircuitBreakerConfiguration<CircuitBreakerConfiguration<TResult>, TResult>
    {
        private Func<TResult, bool> resultPolicy;

        public CircuitBreakerConfiguration<TResult> BrakeWhenExceptionOccurs(Func<Exception, bool> exceptionPolicy)
        {
            base.ExceptionPolicy = exceptionPolicy;
            return this;
        }

        public CircuitBreakerConfiguration<TResult> BrakeWhenResultIs(Func<TResult, bool> resultPolicy)
        {
            this.resultPolicy = resultPolicy;
            return this;
        }

        public CircuitBreakerConfiguration<TResult> WithStateStore(ICircuitStateStore stateStore)
        {
            base.StateStore = stateStore;
            return this;
        }

        public CircuitBreakerConfiguration<TResult> OnOpen(Action<TimeSpan> openHandler)
        {
            base.OpenStateHandler = openHandler;
            return this;
        }

        public CircuitBreakerConfiguration<TResult> OnClosed(Action closedHandler)
        {
            base.ClosedStateHandler = closedHandler;
            return this;
        }

        public CircuitBreakerConfiguration<TResult> OnHalfOpen(Action halfOpenHandler)
        {
            base.HalfOpenStateHandler = halfOpenHandler;
            return this;
        }

        internal bool AcceptsResult(TResult result) =>
            !this.resultPolicy?.Invoke(result) ?? true;

    }
}