using System;

namespace Trybot.CircuitBreaker
{
    public interface ICircuitBreakerConfiguration<out TConfiguration>
    {
        TConfiguration BrakeWhenExceptionOccurs(Func<Exception, bool> exceptionPolicy);

        TConfiguration WithStateStore(ICircuitStateStore stateStore);
    }

    public interface ICircuitBreakerConfiguration<out TConfiguration, TResult> : ICircuitBreakerConfiguration<TConfiguration>
    {
        TConfiguration BrakeWhenResultIs(Func<TResult, bool> resultPolicy);
    }
}