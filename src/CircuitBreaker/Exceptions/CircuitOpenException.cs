using System;

namespace Trybot.CircuitBreaker.Exceptions
{
    public class CircuitOpenException : Exception
    {
        public TimeSpan RemainingOpenTime { get; }
        
        public CircuitOpenException(string message, TimeSpan remainingOpenTime) : base(message)
        {
            this.RemainingOpenTime = remainingOpenTime;
        }
    }
}