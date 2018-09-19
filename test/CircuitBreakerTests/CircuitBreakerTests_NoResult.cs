using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.CircuitBreaker;
using Trybot.CircuitBreaker.Exceptions;

namespace Trybot.Tests.CircuitBreakerTests
{
    [TestClass]
    public class CircuitBreakerTestsNoResult
    {
        enum State
        {
            Open,
            Closed,
            HalfOpen
        }

        private IBotPolicy CreatePolicy(CircuitBreakerConfiguration conf, DefaultCircuitBreakerStrategyConfiguration strConf) =>
            new BotPolicy(config => config
                .Configure(botconfig => botconfig
                    .CircuitBreaker(conf, strConf)));

        private CircuitBreakerConfiguration CreateConfiguration() =>
            new CircuitBreakerConfiguration()
                .BrakeWhenExceptionOccurs(ex => true);

        private DefaultCircuitBreakerStrategyConfiguration CreateStrategyConfiguration(int failure, int success, TimeSpan openDuration) =>
            new DefaultCircuitBreakerStrategyConfiguration()
                .FailureThresholdBeforeOpen(failure)
                .SuccessThresholdInHalfOpen(success)
                .DurationOfOpen(openDuration);

        [TestMethod]
        public void CircuitBreakerTests_Ok()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration(), this.CreateStrategyConfiguration(2, 2, TimeSpan.FromMilliseconds(200)));
            var counter = 0;
            policy.Execute((ex, t) => counter++, CancellationToken.None);

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public async Task CircuitBreakerTests_Ok_Async()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration(), this.CreateStrategyConfiguration(2, 2, TimeSpan.FromMilliseconds(200)));
            var counter = 0;
            await policy.ExecuteAsync((ex, t) => counter++, CancellationToken.None);

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public async Task CircuitBreakerTests_Ok_Async_Task()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration(), this.CreateStrategyConfiguration(2, 2, TimeSpan.FromMilliseconds(200)));
            var counter = 0;
            await policy.ExecuteAsync((ex, t) =>
            {
                counter++;
                return Task.FromResult(0);
            }, CancellationToken.None);

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void CircuitBreakerTests_Brake_Then_Close()
        {
            var state = State.Closed;

            var policy = this.CreatePolicy(this.CreateConfiguration()
                .OnClosed(() => state = State.Closed)
                .OnHalfOpen(() => state = State.HalfOpen)
                .OnOpen(ts =>
                {
                    state = State.Open;
                    Assert.AreEqual(TimeSpan.FromMilliseconds(200), ts);
                }), this.CreateStrategyConfiguration(2, 2, TimeSpan.FromMilliseconds(200)));
            var counter = 0;

            // brake the circuit
            for (var i = 0; i++ < 2;)
                Assert.ThrowsException<InvalidOperationException>(() =>
                    policy.Execute((ctx, t) => throw new InvalidOperationException(), CancellationToken.None));

            Assert.AreEqual(State.Open, state);

            var openException = Assert.ThrowsException<CircuitOpenException>(() =>
                policy.Execute((ctx, t) => { }, CancellationToken.None));

            Assert.IsTrue(openException.RemainingOpenTime <= TimeSpan.FromMilliseconds(200) &&
                          openException.RemainingOpenTime > TimeSpan.FromMilliseconds(100));

            // wait until the open duration is being expired
            Thread.Sleep(openException.RemainingOpenTime.Add(TimeSpan.FromMilliseconds(10)));

            // two synchronous calls will close the circuit
            policy.Execute((ctx, t) =>
            {
                counter++;
                Assert.AreEqual(State.HalfOpen, state);
            }, CancellationToken.None);

            policy.Execute((ctx, t) =>
            {
                counter++;
                Assert.AreEqual(State.HalfOpen, state);
            }, CancellationToken.None);

            Assert.AreEqual(State.Closed, state);

            Assert.AreEqual(2, counter);
        }

        [TestMethod]
        public void CircuitBreakerTests_Brake_Then_Only_Allow_One_Execution_On_HalfOpen()
        {
            var state = State.Closed;

            var policy = this.CreatePolicy(this.CreateConfiguration()
                .OnClosed(() => state = State.Closed)
                .OnHalfOpen(() => state = State.HalfOpen)
                .OnOpen(ts =>
                {
                    state = State.Open;
                    Assert.AreEqual(TimeSpan.FromMilliseconds(200), ts);
                }), this.CreateStrategyConfiguration(2, 2, TimeSpan.FromMilliseconds(200)));
            var counter = 0;

            // brake the circuit
            for (var i = 0; i++ < 2;)
                Assert.ThrowsException<InvalidOperationException>(() =>
                    policy.Execute((ctx, t) => throw new InvalidOperationException(), CancellationToken.None));

            Assert.AreEqual(State.Open, state);

            var openException = Assert.ThrowsException<CircuitOpenException>(() =>
                policy.Execute((ctx, t) => { }, CancellationToken.None));

            // wait until the open duration is being expired
            Thread.Sleep(openException.RemainingOpenTime.Add(TimeSpan.FromMilliseconds(10)));

            // simulate fast parallel calls, the faster one will win and attempts to close the circuit, the other one will be rejected
            Parallel.For(0, 2, (i, s) =>
                {
                    try
                    {
                        policy.Execute((ctx, t) =>
                        {
                            counter++;
                            Assert.AreEqual(State.HalfOpen, state);
                        }, CancellationToken.None);

                    }
                    catch (Exception e)
                    {
                        Assert.IsInstanceOfType(e, typeof(HalfOpenExecutionLimitExceededException));
                    }
                });

            Assert.AreEqual(State.HalfOpen, state);

            Assert.AreEqual(1, counter);

            // next call will close the circuit
            policy.Execute((ctx, t) =>
            {
                counter++;
                Assert.AreEqual(State.HalfOpen, state);
            }, CancellationToken.None);

            Assert.AreEqual(State.Closed, state);

            Assert.AreEqual(2, counter);
        }
    }
}
