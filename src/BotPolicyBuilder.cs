using System;
using Trybot.Utils;

namespace Trybot
{
    internal class BotPolicyBuilder : IBotPolicyBuilder
    {
        public Bot Bot { get; private set; } = new DefaultBot();

        public IBotPolicyBuilder AddBot<TBot, TConfiguration>(Func<Bot, TConfiguration, TBot> factory, Action<TConfiguration> configuratorAction)
            where TBot : ConfigurableBot<TConfiguration>
            where TConfiguration : new()
        {
            Shield.EnsureNotNull(factory, nameof(factory));
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));

            var configuration = Activator.CreateInstance<TConfiguration>();
            configuratorAction(configuration);

            this.Bot = factory(this.Bot, configuration);
            return this;
        }

        public IBotPolicyBuilder AddBot<TBot, TConfiguration>(Func<Bot, TConfiguration, TBot> factory, TConfiguration configuration)
            where TBot : ConfigurableBot<TConfiguration>
        {
            Shield.EnsureNotNull(factory, nameof(factory));

            this.Bot = factory(this.Bot, configuration);
            return this;
        }

        public IBotPolicyBuilder AddBot<TBot>(Func<Bot, TBot> factory)
            where TBot : Bot
        {
            Shield.EnsureNotNull(factory, nameof(factory));

            this.Bot = factory(this.Bot);
            return this;
        }
    }

    internal class BotPolicyBuilder<TResult> : IBotPolicyBuilder<TResult>
    {
        public Bot<TResult> Bot { get; private set; } = new DefaultBot<TResult>();

        public IBotPolicyBuilder<TResult> AddBot<TBot, TConfiguration>(Func<Bot<TResult>, TConfiguration, TBot> factory, Action<TConfiguration> configuratorAction)
            where TBot : ConfigurableBot<TConfiguration, TResult>
            where TConfiguration : new()
        {
            Shield.EnsureNotNull(factory, nameof(factory));
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));

            var configuration = Activator.CreateInstance<TConfiguration>();
            configuratorAction(configuration);

            this.Bot = factory(this.Bot, configuration);

            return this;
        }

        public IBotPolicyBuilder<TResult> AddBot<TBot, TConfiguration>(Func<Bot<TResult>, TConfiguration, TBot> factory, TConfiguration configuration)
            where TBot : ConfigurableBot<TConfiguration, TResult>
        {
            Shield.EnsureNotNull(factory, nameof(factory));

            this.Bot = factory(this.Bot, configuration);
            return this;
        }

        public IBotPolicyBuilder<TResult> AddBot<TBot>(Func<Bot<TResult>, TBot> factory)
            where TBot : Bot<TResult>
        {
            Shield.EnsureNotNull(factory, nameof(factory));

            this.Bot = factory(this.Bot);
            return this;
        }
    }
}
