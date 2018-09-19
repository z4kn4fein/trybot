using System;

namespace Trybot.CircuitBreaker.Exceptions
{
    public class HalfOpenExecutionLimitExceededException : Exception
    {
        public HalfOpenExecutionLimitExceededException(string message) : base(message)
        {
        }
    }
}
