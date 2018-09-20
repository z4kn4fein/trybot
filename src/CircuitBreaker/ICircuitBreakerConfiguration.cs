using System;

namespace Trybot.CircuitBreaker
{
    /// <summary>
    /// Represents the fluent api of the circuit breaker configuration.
    /// </summary>
    public interface ICircuitBreakerConfiguration<out TConfiguration>
    {
        TConfiguration BrakeWhenExceptionOccurs(Func<Exception, bool> exceptionPolicy);

        TConfiguration WithStateStore(ICircuitStateStore stateStore);

        TConfiguration OnOpen(Action<TimeSpan> openHandler);

        TConfiguration OnClosed(Action closedHandler);

        TConfiguration OnHalfOpen(Action halfOpenHandler);
    }

    /// <summary>
    /// Represents the fluent api of the circuit breaker configuration.
    /// </summary>
    public interface ICircuitBreakerConfiguration<out TConfiguration, out TResult> : ICircuitBreakerConfiguration<TConfiguration>
    {
        TConfiguration BrakeWhenResultIs(Func<TResult, bool> resultPolicy);
    }
}