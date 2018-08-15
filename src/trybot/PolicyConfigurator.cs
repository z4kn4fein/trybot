using System;

namespace Trybot
{
    internal class PolicyConfigurator : IPolicyConfigurator
    {
        public Bot Bot { get; private set; } = new DefaultBot();

        public IPolicyConfigurator AddBot<TBot, TConfiguration>(Func<Bot, TConfiguration, TBot> factory, Action<TConfiguration> configuratorAction)
            where TBot : Bot<TConfiguration>
            where TConfiguration : new()
        {
            var configuration = Activator.CreateInstance<TConfiguration>();
            configuratorAction(configuration);

            this.Bot = factory(this.Bot, configuration);

            return this;
        }

        public IPolicyConfigurator AddBot<TBot>(Func<Bot, TBot> factory)
            where TBot : Bot
        {
            this.Bot = factory(this.Bot);
            return this;
        }
    }
}
