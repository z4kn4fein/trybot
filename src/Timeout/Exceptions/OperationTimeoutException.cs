using System;

namespace Trybot.Timeout.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an exception which will be thrown by the <see cref="IBotPolicy" /> when a configured timeout bot triggers.
    /// </summary>
    public class OperationTimeoutException : Exception
    {
        /// <inheritdoc />
        public OperationTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
