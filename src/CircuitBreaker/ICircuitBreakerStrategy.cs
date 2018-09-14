using System;

namespace Trybot.CircuitBreaker
{
    public interface ICircuitBreakerStrategy
    {
        bool IsCircuitBroken();

        TimeSpan GetTimeUntilHalfOpenState();

        void OperationSucceeded();

        void OperationFailed();
    }
}
