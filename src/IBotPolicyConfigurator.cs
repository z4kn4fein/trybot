using System;
using System.Threading.Tasks;

namespace Trybot
{
    /// <summary>
    /// Represents a bot policy configurator which can be used to configure a given <see cref="IBotPolicy"/>.
    /// </summary>
    /// <typeparam name="TConfigurator">The type of the implementation to return, gives the api a fluent design.</typeparam>
    public interface IBotPolicyConfiguratorBase<out TConfigurator>
    {
        /// <summary>
        /// Sets the value passed to the <see cref="Task.ConfigureAwait"/> method in case of awaited asynchronous calls.
        /// </summary>
        /// <param name="continueOnCapturedContext">The value passed to the <see cref="Task.ConfigureAwait"/> method.</param>
        /// <returns>The implementation of this interface.</returns>
        TConfigurator SetCapturedContextContinuation(bool continueOnCapturedContext = false);
    }

    /// <inheritdoc />
    /// <summary>
    /// Configures a <see cref="IBotPolicy{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">The result type of the given operation passed to the <see cref="IBotPolicy{TResult}"/>.</typeparam>
    public interface IBotPolicyConfigurator<TResult> : IBotPolicyConfiguratorBase<IBotPolicyConfigurator<TResult>>
    {
        /// <summary>
        /// Used to set up an <see cref="IBotPolicy{TResult}"/> with the given <see cref="IBotPolicyBuilder{TResult}"/>.
        /// </summary>
        /// <param name="policyConfigurator">The configurator action.</param>
        /// <returns>The implementation of this interface.</returns>
        IBotPolicyConfigurator<TResult> Configure(Action<IBotPolicyBuilder<TResult>> policyConfigurator);
    }

    /// <inheritdoc />
    /// <summary>
    /// Configures a <see cref="T:Trybot.IBotPolicy" />.
    /// </summary>
    public interface IBotPolicyConfigurator : IBotPolicyConfiguratorBase<IBotPolicyConfigurator>
    {
        /// <summary>
        /// Used to set up an <see cref="IBotPolicy"/> with the given <see cref="IBotPolicyBuilder"/>.
        /// </summary>
        /// <param name="policyConfigurator">The configurator action.</param>
        /// <returns>The implementation of this interface.</returns>
        IBotPolicyConfigurator Configure(Action<IBotPolicyBuilder> policyConfigurator);
    }
}
