using System;

namespace Trybot
{
    public interface IExecutorConfigurator
    {
        IExecutorConfigurator SetCapturedContextContinuation(bool continueOnCapturedContext = false);

        IExecutorConfigurator DefinePolicy(object identifier, Action<IPolicyConfigurator> policyConfigurator);
    }
}
