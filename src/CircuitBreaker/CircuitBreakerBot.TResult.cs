using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot.CircuitBreaker
{
    public class CircuitBreakerBot<TResult> : ConfigurableBot<CircuitBreakerConfiguration<TResult>, TResult>
    {
        private readonly CircuitBreakerController controller;

        internal CircuitBreakerBot(Bot<TResult> innerBot, CircuitBreakerConfiguration<TResult> configuration, Func<ICircuitBreakerStateSwitcher, ICircuitBreakerStrategy> strategyFactory) : base(innerBot, configuration)
        {
            this.controller = new CircuitBreakerController(strategyFactory, base.Configuration.StateStore);
        }

        public override TResult Execute(IBotOperation<TResult> operation, ExecutionContext context, CancellationToken token)
        {
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
        }

        public override async Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, ExecutionContext context, CancellationToken token)
        {
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
        }
    }
}
