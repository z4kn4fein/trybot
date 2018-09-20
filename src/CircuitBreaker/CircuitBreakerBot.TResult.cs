using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.CircuitBreaker.Exceptions;
using Trybot.Operations;
using Trybot.Utils;

namespace Trybot.CircuitBreaker
{
    internal class CircuitBreakerBot<TResult> : ConfigurableBot<CircuitBreakerConfiguration<TResult>, TResult>
    {
        private readonly CircuitBreakerController controller;
        private readonly AtomicBool executionBarrier;

        internal CircuitBreakerBot(Bot<TResult> innerBot, CircuitBreakerConfiguration<TResult> configuration, Func<ICircuitBreakerStateSwitcher, ICircuitBreakerStrategy> strategyFactory) : base(innerBot, configuration)
        {
            this.controller = new CircuitBreakerController(strategyFactory, base.Configuration);
            this.executionBarrier = new AtomicBool();
        }

        public override TResult Execute(IBotOperation<TResult> operation, ExecutionContext context, CancellationToken token)
        {
            var shouldLimitExecution = this.controller.PreCheckCircuitState();
            if (shouldLimitExecution && !this.executionBarrier.CompareExchange(false, true))
                throw new HalfOpenExecutionLimitExceededException(Constants.HalfOpenExecutionLimitExceededExceptionMessage);

            try
            {
                var result = base.InnerBot.Execute(operation, context, token);

                if (base.Configuration.AcceptsResult(result))
                    this.controller.OperationSucceeded();
                else
                    this.controller.OperationFailed();

                return result;
            }
            catch (Exception exception)
            {
                if (base.Configuration.HandlesException(exception))
                    this.controller.OperationFailed();

                throw;
            }
            finally
            {
                this.executionBarrier.SetValue(false);
            }
        }

        public override async Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, ExecutionContext context, CancellationToken token)
        {
            var shouldLimitExecution = this.controller.PreCheckCircuitState();
            if (shouldLimitExecution && !this.executionBarrier.CompareExchange(false, true))
                throw new HalfOpenExecutionLimitExceededException(Constants.HalfOpenExecutionLimitExceededExceptionMessage);

            try
            {
                var result = await base.InnerBot.ExecuteAsync(operation, context, token);
                if (base.Configuration.AcceptsResult(result))
                    this.controller.OperationSucceeded();
                else
                    this.controller.OperationFailed();

                return result;
            }
            catch (Exception exception)
            {
                if (base.Configuration.HandlesException(exception))
                    this.controller.OperationFailed();

                throw;
            }
            finally
            {
                this.executionBarrier.SetValue(false);
            }
        }
    }
}
