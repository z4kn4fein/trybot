using System.Threading;
using System.Threading.Tasks;

namespace Trybot.CircuitBreaker
{
    /// <summary>
    /// Represents a circuit breaker strategy used by the circuit bot internally.
    /// It defines the actual behavior of the circuit breaker based on the given operation related events.
    /// </summary>
    public interface ICircuitBreakerStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool PreCheckCircuitState();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> PreCheckCircuitStateAsync(CancellationToken token, bool continueOnCapturedContext);

        /// <summary>
        /// 
        /// </summary>
        void OperationSucceeded();

        /// <summary>
        /// 
        /// </summary>
        Task OperationSucceededAsync(CancellationToken token, bool continueOnCapturedContext);

        /// <summary>
        /// 
        /// </summary>
        void OperationFailed();

        /// <summary>
        /// 
        /// </summary>
        Task OperationFailedAsync(CancellationToken token, bool continueOnCapturedContext);
    }
}
