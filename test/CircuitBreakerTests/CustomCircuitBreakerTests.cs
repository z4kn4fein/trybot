using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using Trybot.CircuitBreaker;

namespace Trybot.Tests.CircuitBreakerTests
{
    [TestClass]
    public class CustomCircuitBreakerTests
    {
        private class CustomStrategy : CircuitBreakerStrategy
        {
            public CustomStrategy(CircuitBreakerConfigurationBase configuration) : base(configuration)
            { }

            protected override bool OperationFailedInClosed() => true;

            protected override bool OperationFailedInHalfOpen() => true;

            protected override bool OperationSucceededInHalfOpen() => true;

            protected override void Reset() { }
        }

        [TestMethod]
        public void CustomCircuitBreakerTests_Closed_Open_HalfOpen_Then_Closed()
        {
            var state = State.Closed;
            var policy = new BotPolicy(config => config
                .Configure(botConfig => botConfig
                    .CustomCircuitBreaker(cbConfig => new CustomStrategy(cbConfig),
                        cbConfig => cbConfig.BrakeWhenExceptionOccurs(ex => true)
                        .OnClosed(() => state = State.Closed)
                        .OnHalfOpen(() => state = State.HalfOpen)
                        .OnOpen(ts => state = State.Open))));

            Assert.ThrowsException<InvalidOperationException>(() =>
                policy.Execute((ctx, t) => throw new InvalidOperationException(), CancellationToken.None));

            Assert.AreEqual(State.Open, state);

            policy.Execute((ctx, t) => Assert.AreEqual(State.HalfOpen, state), CancellationToken.None);

            Assert.AreEqual(State.Closed, state);
        }

        [TestMethod]
        public void CustomCircuitBreakerTests_Closed_Open_HalfOpen_Then_Closed_WithConfig()
        {
            var state = State.Closed;
            var policy = new BotPolicy(config => config
                .Configure(botConfig => botConfig
                    .CustomCircuitBreaker(cbConfig => new CustomStrategy(cbConfig),
                        new CircuitBreakerConfiguration().BrakeWhenExceptionOccurs(ex => true)
                            .OnClosed(() => state = State.Closed)
                            .OnHalfOpen(() => state = State.HalfOpen)
                            .OnOpen(ts => state = State.Open))));

            Assert.ThrowsException<InvalidOperationException>(() =>
                policy.Execute((ctx, t) => throw new InvalidOperationException(), CancellationToken.None));

            Assert.AreEqual(State.Open, state);

            policy.Execute((ctx, t) => Assert.AreEqual(State.HalfOpen, state), CancellationToken.None);

            Assert.AreEqual(State.Closed, state);
        }

        [TestMethod]
        public void CustomCircuitBreakerTests_Result_Closed_Open_HalfOpen_Then_Closed()
        {
            var state = State.Closed;
            var policy = new BotPolicy<int>(config => config
                .Configure(botConfig => botConfig
                    .CustomCircuitBreaker(cbConfig => new CustomStrategy(cbConfig),
                        cbConfig => cbConfig.BrakeWhenResultIs(r => r != 5)
                            .OnClosed(() => state = State.Closed)
                            .OnHalfOpen(() => state = State.HalfOpen)
                            .OnOpen(ts => state = State.Open))));

            policy.Execute((ctx, t) => 6, CancellationToken.None);

            Assert.AreEqual(State.Open, state);

            policy.Execute((ctx, t) =>
            {
                Assert.AreEqual(State.HalfOpen, state);
                return 5;
            }, CancellationToken.None);

            Assert.AreEqual(State.Closed, state);
        }

        [TestMethod]
        public void CustomCircuitBreakerTests_Result_Closed_Open_HalfOpen_Then_Closed_WithConfig()
        {
            var state = State.Closed;
            var policy = new BotPolicy<int>(config => config
                .Configure(botConfig => botConfig
                    .CustomCircuitBreaker(cbConfig => new CustomStrategy(cbConfig),
                        new CircuitBreakerConfiguration<int>().BrakeWhenResultIs(r => r != 5)
                            .OnClosed(() => state = State.Closed)
                            .OnHalfOpen(() => state = State.HalfOpen)
                            .OnOpen(ts => state = State.Open))));

            policy.Execute((ctx, t) => 6, CancellationToken.None);

            Assert.AreEqual(State.Open, state);

            policy.Execute((ctx, t) =>
            {
                Assert.AreEqual(State.HalfOpen, state);
                return 5;
            }, CancellationToken.None);

            Assert.AreEqual(State.Closed, state);
        }
    }
}
