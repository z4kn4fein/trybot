using System;

namespace Trybot.CircuitBreaker.Exceptions
{
    /// <summary>
    /// Represents the exception which will be thrown by the circuit breaker bot
    /// when more than one operations is executed while the circuit is in half open state.
    /// </summary>
    public class HalfOpenExecutionLimitExceededException : Exception
    {
        internal HalfOpenExecutionLimitExceededException(string message) : base(message)
        {
        }
    }
}
