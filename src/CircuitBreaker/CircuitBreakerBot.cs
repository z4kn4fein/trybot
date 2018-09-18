using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot.CircuitBreaker
{
    public class CircuitBreakerBot : ConfigurableBot<CircuitBreakerConfiguration>
    {
        private readonly CircuitBreakerController controller;

        internal CircuitBreakerBot(Bot innerBot, CircuitBreakerConfiguration configuration, Func<ICircuitBreakerStateSwitcher, ICircuitBreakerStrategy> strategyFactory) : base(innerBot, configuration)
        {
            this.controller = new CircuitBreakerController(strategyFactory, base.Configuration.StateStore);
        }

        public override void Execute(IBotOperation operation, ExecutionContext context, CancellationToken token)
        {
            try
            {
                base.InnerBot.Execute(operation, context, token);
                this.controller.OperationSucceeded();
            }
            catch (Exception exception)
            {
                if(base.Configuration.HandlesException(exception))
                    this.controller.OperationFailed();
                
                throw;
            }
        }

        public override async Task ExecuteAsync(IAsyncBotOperation operation, ExecutionContext context, CancellationToken token)
        {
            try
            {
                await base.InnerBot.ExecuteAsync(operation, context, token);
                this.controller.OperationSucceeded();
            }
            catch (Exception exception)
            {
                if(base.Configuration.HandlesException(exception))
                    this.controller.OperationFailed();
                
                throw;
            }
        }
    }
}