using System;

namespace Trybot.CircuitBreaker.Exceptions
{
    /// <summary>
    /// Represents the exception which will be thrown by the circuit breaker bot
    /// when an operation is executed while the circuit is in open state.
    /// </summary>
    public class CircuitOpenException : Exception
    {
        /// <summary>
        /// The time until the circuit remains open.
        /// </summary>
        public TimeSpan RemainingOpenTime { get; }
        
        internal CircuitOpenException(string message, TimeSpan remainingOpenTime) : base(message)
        {
            this.RemainingOpenTime = remainingOpenTime;
        }
    }
}