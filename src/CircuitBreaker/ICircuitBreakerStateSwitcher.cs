using System;

namespace Trybot.CircuitBreaker
{
    public interface ICircuitBreakerStateSwitcher
    {
        void Close();

        void Open(TimeSpan duration);
    }
}
