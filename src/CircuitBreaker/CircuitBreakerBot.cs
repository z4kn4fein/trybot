using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.CircuitBreaker.Exceptions;
using Trybot.Operations;
using Trybot.Utils;

namespace Trybot.CircuitBreaker
{
    internal class CircuitBreakerBot : ConfigurableBot<CircuitBreakerConfiguration>
    {
        private readonly CircuitBreakerController controller;
        private readonly AtomicBool executionBarrier;

        internal CircuitBreakerBot(Bot innerBot, CircuitBreakerConfiguration configuration, Func<ICircuitBreakerStateSwitcher, ICircuitBreakerStrategy> strategyFactory) : base(innerBot, configuration)
        {
            this.controller = new CircuitBreakerController(strategyFactory, base.Configuration);
            this.executionBarrier = new AtomicBool();
        }

        public override void Execute(IBotOperation operation, ExecutionContext context, CancellationToken token)
        {
            var shouldLimitExecution = this.controller.PreCheckCircuitState();
            if (shouldLimitExecution && !this.executionBarrier.CompareExchange(false, true))
                throw new HalfOpenExecutionLimitExceededException(Constants.HalfOpenExecutionLimitExceededExceptionMessage);

            try
            {
                base.InnerBot.Execute(operation, context, token);
                this.controller.OperationSucceeded();
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

        public override async Task ExecuteAsync(IAsyncBotOperation operation, ExecutionContext context, CancellationToken token)
        {
            var shouldLimitExecution = this.controller.PreCheckCircuitState();
            if (shouldLimitExecution && !this.executionBarrier.CompareExchange(false, true))
                throw new HalfOpenExecutionLimitExceededException(Constants.HalfOpenExecutionLimitExceededExceptionMessage);

            try
            {
                await base.InnerBot.ExecuteAsync(operation, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                this.controller.OperationSucceeded();
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