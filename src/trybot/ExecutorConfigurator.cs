using System;
using Trybot.Utils;

namespace Trybot
{
    internal class ExecutorConfigurator : IExecutorConfigurator
    {
        public ExecutorConfiguration Configuration { get; } = new ExecutorConfiguration();

        public AvlTreeKeyValue<object, Bot> Policies { get; private set; } = AvlTreeKeyValue<object, Bot>.Empty;

        public IExecutorConfigurator SetCapturedContextContinuation(bool continueOnCapturedContext = false)
        {
            this.Configuration.ContinueOnCapturedContext = continueOnCapturedContext;
            return this;
        }

        public IExecutorConfigurator DefinePolicy(object identifier, Action<IPolicyConfigurator> configuratorAction)
        {
            var configurator = new PolicyConfigurator();
            configuratorAction(configurator);

            this.Policies = this.Policies.AddOrUpdate(identifier, configurator.Bot);

            return this;
        }
    }
}
