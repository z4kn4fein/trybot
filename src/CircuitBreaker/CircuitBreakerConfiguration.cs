using System;

namespace Trybot.CircuitBreaker
{
    public class CircuitBreakerConfigurationBase
    {
        protected Func<Exception, bool> ExceptionPolicy { get; set; }

        internal ICircuitStateStore StateStore { get; set; }

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

        internal bool AcceptsResult(TResult result) =>
            !this.resultPolicy?.Invoke(result) ?? true;

    }
}