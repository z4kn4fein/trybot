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
        private readonly CircuitBreakerStrategy strategy;
        private readonly AtomicBool executionBarrier;

        internal CircuitBreakerBot(Bot innerBot, CircuitBreakerConfiguration configuration, CircuitBreakerStrategy strategy) : base(innerBot, configuration)
        {
            this.strategy = strategy;
            this.executionBarrier = new AtomicBool();
        }

        public override void Execute(IBotOperation operation, ExecutionContext context, CancellationToken token)
        {
            var shouldLimitExecution = this.strategy.PreCheckCircuitState();
            if (shouldLimitExecution && !this.executionBarrier.CompareExchange(false, true))
                throw new HalfOpenExecutionLimitExceededException(Constants.HalfOpenExecutionLimitExceededExceptionMessage);

            try
            {
                base.InnerBot.Execute(operation, context, token);
                this.strategy.OperationSucceeded();
            }
            catch (Exception exception)
            {
                if (base.Configuration.HandlesException(exception))
                    this.strategy.OperationFailed();

                throw;
            }
            finally
            {
                this.executionBarrier.SetValue(false);
            }
        }

        public override async Task ExecuteAsync(IAsyncBotOperation operation, ExecutionContext context, CancellationToken token)
        {
            var shouldLimitExecution = await this.strategy.PreCheckCircuitStateAsync(token, context.BotPolicyConfiguration.ContinueOnCapturedContext)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

            if (shouldLimitExecution && !this.executionBarrier.CompareExchange(false, true))
                throw new HalfOpenExecutionLimitExceededException(Constants.HalfOpenExecutionLimitExceededExceptionMessage);

            try
            {
                await base.InnerBot.ExecuteAsync(operation, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                await this.strategy.OperationSucceededAsync(token, context.BotPolicyConfiguration.ContinueOnCapturedContext)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
            }
            catch (Exception exception)
            {
                if (base.Configuration.HandlesException(exception))
                    await this.strategy.OperationFailedAsync(token, context.BotPolicyConfiguration.ContinueOnCapturedContext)
                        .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                throw;
            }
            finally
            {
                this.executionBarrier.SetValue(false);
            }
        }
    }
}