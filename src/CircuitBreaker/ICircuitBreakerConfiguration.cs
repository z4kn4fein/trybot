using System;

namespace Trybot.CircuitBreaker
{
    public interface ICircuitBreakerConfiguration<out TConfiguration>
    {
        TConfiguration BrakeWhenExceptionOccurs(Func<Exception, bool> exceptionPolicy);

        TConfiguration WithStateStore(ICircuitStateStore stateStore);

        TConfiguration OnOpen(Action<TimeSpan> openHandler);

        TConfiguration OnClosed(Action closedHandler);

        TConfiguration OnHalfOpen(Action halfOpenHandler);
    }

    public interface ICircuitBreakerConfiguration<out TConfiguration, TResult> : ICircuitBreakerConfiguration<TConfiguration>
    {
        TConfiguration BrakeWhenResultIs(Func<TResult, bool> resultPolicy);
    }
}