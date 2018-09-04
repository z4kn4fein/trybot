using System;

namespace Trybot
{
    public interface IBotPolicyBuilder
    {
        IBotPolicyBuilder AddBot<TBot, TConfiguration>(Func<Bot, TConfiguration, TBot> factory, Action<TConfiguration> configuratorAction)
            where TBot : ConfigurableBot<TConfiguration>
            where TConfiguration : new();

        IBotPolicyBuilder AddBot<TBot>(Func<Bot, TBot> factory)
            where TBot : Bot;
    }

    public interface IBotPolicyBuilder<TResult>
    {
        IBotPolicyBuilder<TResult> AddBot<TBot, TConfiguration>(Func<Bot<TResult>, TConfiguration, TBot> factory, Action<TConfiguration> configuratorAction)
            where TBot : ConfigurableBot<TConfiguration, TResult>
            where TConfiguration : new();

        IBotPolicyBuilder<TResult> AddBot<TBot>(Func<Bot<TResult>, TBot> factory)
            where TBot : Bot<TResult>;
    }
}
