using System;

namespace Trybot
{
    public interface IBotPolicyConfiguratorBase<out TConfigurator>
    {
        TConfigurator SetCapturedContextContinuation(bool continueOnCapturedContext = false);
    }

    public interface IBotPolicyConfigurator<TResult> : IBotPolicyConfiguratorBase<IBotPolicyConfigurator<TResult>>
    {
        IBotPolicyConfigurator<TResult> Configure(Action<IBotPolicyBuilder<TResult>> policyConfigurator);
    }

    public interface IBotPolicyConfigurator : IBotPolicyConfiguratorBase<IBotPolicyConfigurator>
    {
        IBotPolicyConfigurator Configure(Action<IBotPolicyBuilder> policyConfigurator);
    }
}
