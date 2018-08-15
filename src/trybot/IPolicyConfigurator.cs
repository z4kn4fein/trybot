using System;

namespace Trybot
{
    public interface IPolicyConfigurator
    {
        IPolicyConfigurator AddBot<TBot, TConfiguration>(Func<Bot, TConfiguration, TBot> factory, Action<TConfiguration> configuratorAction)
            where TBot : Bot<TConfiguration>
            where TConfiguration : new();

        IPolicyConfigurator AddBot<TBot>(Func<Bot, TBot> factory)
            where TBot : Bot;
    }
}
