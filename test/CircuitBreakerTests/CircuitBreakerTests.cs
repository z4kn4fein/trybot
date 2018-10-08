using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Trybot.CircuitBreaker;
using Trybot.CircuitBreaker.Exceptions;

namespace Trybot.Tests.CircuitBreakerTests
{
    [TestClass]
    public class CircuitBreakerTests
    {
        private IBotPolicy<TResult> CreatePolicy<TResult>(CircuitBreakerConfiguration<TResult> conf, DefaultCircuitBreakerStrategyConfiguration strConf) =>
            new BotPolicy<TResult>(config => config
                .Configure(botconfig => botconfig
                    .CircuitBreaker(conf, strConf)));

        private CircuitBreakerConfiguration<TResult> CreateConfiguration<TResult>(TimeSpan openDuration) =>
            new CircuitBreakerConfiguration<TResult>()
                .BrakeWhenExceptionOccurs(ex => true)
                .DurationOfOpen(openDuration);

        private DefaultCircuitBreakerStrategyConfiguration CreateStrategyConfiguration(int failure, int success) =>
            new DefaultCircuitBreakerStrategyConfiguration()
                .FailureThresholdBeforeOpen(failure)
                .SuccessThresholdInHalfOpen(success);

        [TestMethod]
        public void CircuitBreakerTests_Ok()
        {
            var mockStore = new Mock<ICircuitStateHandler>();
            mockStore.Setup(s => s.Read()).Returns(CircuitState.Closed).Verifiable();
            var policy = new BotPolicy<int>(config => config
                .Configure(botconfig => botconfig
                    .CircuitBreaker(conf => conf
                    .WithStateHandler(mockStore.Object)
                    .DurationOfOpen(TimeSpan.FromMilliseconds(200)),
                        defConfig => defConfig
                            .FailureThresholdBeforeOpen(2)
                            .SuccessThresholdInHalfOpen(2))));
            var counter = 0;
            policy.Execute((ex, t) => counter++, CancellationToken.None);

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public async Task CircuitBreakerTests_Ok_Async()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>(TimeSpan.FromMilliseconds(200)),
                this.CreateStrategyConfiguration(2, 2));
            var counter = 0;
            await policy.ExecuteAsync((ex, t) => counter++, CancellationToken.None);

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public async Task CircuitBreakerTests_Ok_Async_Task()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>(TimeSpan.FromMilliseconds(200)),
                this.CreateStrategyConfiguration(2, 2));
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
            var successResult = 5;

            var policy = this.CreatePolicy(this.CreateConfiguration<int>(TimeSpan.FromMilliseconds(200))
                .BrakeWhenResultIs(r => r != successResult)
                .OnClosed(() => state = State.Closed)
                .OnHalfOpen(() => state = State.HalfOpen)
                .OnOpen(ts =>
                {
                    state = State.Open;
                    Assert.AreEqual(TimeSpan.FromMilliseconds(200), ts);
                }), this.CreateStrategyConfiguration(2, 2));
            var counter = 0;

            // brake the circuit
            policy.Execute((ctx, t) => successResult + 1, CancellationToken.None);
            Assert.ThrowsException<Exception>(() =>
                policy.Execute((ctx, t) => throw new Exception(), CancellationToken.None));

            Assert.AreEqual(State.Open, state);

            var openException = Assert.ThrowsException<CircuitOpenException>(() =>
                policy.Execute((ctx, t) => 0, CancellationToken.None));

            Assert.IsTrue(openException.RemainingOpenTime <= TimeSpan.FromMilliseconds(200) &&
                          openException.RemainingOpenTime > TimeSpan.FromMilliseconds(100));

            // wait until the open duration is being expired
            Thread.Sleep(openException.RemainingOpenTime.Add(TimeSpan.FromMilliseconds(10)));

            // two calls will close the circuit
            policy.Execute((ctx, t) =>
            {
                counter++;
                Assert.AreEqual(State.HalfOpen, state);
                return successResult;
            }, CancellationToken.None);

            policy.Execute((ctx, t) =>
            {
                counter++;
                Assert.AreEqual(State.HalfOpen, state);
                return successResult;
            }, CancellationToken.None);

            Assert.AreEqual(State.Closed, state);

            Assert.AreEqual(2, counter);
        }

        [TestMethod]
        public async Task CircuitBreakerTests_Brake_Then_Close_Async()
        {
            var state = State.Closed;
            var successResult = 5;

            var policy = this.CreatePolicy(this.CreateConfiguration<int>(TimeSpan.FromMilliseconds(200))
                .BrakeWhenResultIs(r => r != successResult)
                .OnClosed(() => state = State.Closed)
                .OnHalfOpen(() => state = State.HalfOpen)
                .OnOpen(ts =>
                {
                    state = State.Open;
                    Assert.AreEqual(TimeSpan.FromMilliseconds(200), ts);
                }), this.CreateStrategyConfiguration(2, 2));
            var counter = 0;

            // brake the circuit
            await policy.ExecuteAsync((ctx, t) => Task.FromResult(successResult + 1), CancellationToken.None);
            await Assert.ThrowsExceptionAsync<NullReferenceException>(async() => 
                await policy.ExecuteAsync((ctx, t) => {
                        object o = null;
                        o.GetHashCode();
                        return Task.FromResult(0);
                    }, CancellationToken.None));

            Assert.AreEqual(State.Open, state);

            var openException = await Assert.ThrowsExceptionAsync<CircuitOpenException>(() =>
                policy.ExecuteAsync((ctx, t) => 0, CancellationToken.None));

            Assert.IsTrue(openException.RemainingOpenTime <= TimeSpan.FromMilliseconds(200) &&
                          openException.RemainingOpenTime > TimeSpan.FromMilliseconds(100));

            // wait until the open duration is being expired
            Thread.Sleep(openException.RemainingOpenTime.Add(TimeSpan.FromMilliseconds(10)));

            // two calls will close the circuit
            await policy.ExecuteAsync((ctx, t) =>
            {
                counter++;
                Assert.AreEqual(State.HalfOpen, state);
                return Task.FromResult(successResult);
            }, CancellationToken.None);

            await policy.ExecuteAsync((ctx, t) =>
            {
                counter++;
                Assert.AreEqual(State.HalfOpen, state);
                return successResult;
            }, CancellationToken.None);

            Assert.AreEqual(State.Closed, state);

            Assert.AreEqual(2, counter);
        }

        [TestMethod]
        [Ignore] // This test is very undeterministic, used only for local testing
        public void CircuitBreakerTests_Brake_Then_Only_Allow_One_Execution_On_HalfOpen()
        {
            var state = State.Closed;

            var policy = this.CreatePolicy(this.CreateConfiguration<int>(TimeSpan.FromMilliseconds(200))
                .OnClosed(() => state = State.Closed)
                .OnHalfOpen(() => state = State.HalfOpen)
                .OnOpen(ts =>
                {
                    state = State.Open;
                    Assert.AreEqual(TimeSpan.FromMilliseconds(200), ts);
                }), this.CreateStrategyConfiguration(2, 2));
            var counter = 0;

            // brake the circuit
            for (var i = 0; i < 2; i++)
                Assert.ThrowsException<InvalidOperationException>(() =>
                    policy.Execute((ctx, t) => throw new InvalidOperationException(), CancellationToken.None));

            Assert.AreEqual(State.Open, state);

            var openException = Assert.ThrowsException<CircuitOpenException>(() =>
                policy.Execute((ctx, t) => 0, CancellationToken.None));

            // wait until the open duration is being expired
            Thread.Sleep(openException.RemainingOpenTime.Add(TimeSpan.FromMilliseconds(10)));

            // simulate fast parallel calls, the faster one will win and attempts to close the circuit, the other one will be rejected
            Parallel.For(0, 2, i =>
            {
                try
                {
                    policy.Execute((ctx, t) =>
                    {
                        counter++;
                        Assert.AreEqual(State.HalfOpen, state);
                        Task.Delay(TimeSpan.FromMilliseconds(300), t).Wait(t);
                        return 0;
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
                return 0;
            }, CancellationToken.None);

            Assert.AreEqual(State.Closed, state);

            Assert.AreEqual(2, counter);
        }

        [TestMethod]
        [Ignore] // This test is very undeterministic, used only for local testing
        public async Task CircuitBreakerTests_Brake_Then_Only_Allow_One_Execution_On_HalfOpen_Async()
        {
            var state = State.Closed;

            var policy = this.CreatePolicy(this.CreateConfiguration<int>(TimeSpan.FromMilliseconds(200))
                .OnClosed(() => state = State.Closed)
                .OnHalfOpen(() => state = State.HalfOpen)
                .OnOpen(ts =>
                {
                    state = State.Open;
                    Assert.AreEqual(TimeSpan.FromMilliseconds(200), ts);
                }), this.CreateStrategyConfiguration(2, 2));
            var counter = 0;

            // brake the circuit
            for (var i = 0; i < 2; i++)
                await Assert.ThrowsExceptionAsync<NullReferenceException>(() =>
                    policy.ExecuteAsync((ctx, t) =>
                    {
                        object o = null;
                        o.GetHashCode();
                        return Task.FromResult(0);
                    }, CancellationToken.None));

            Assert.AreEqual(State.Open, state);

            var openException = await Assert.ThrowsExceptionAsync<CircuitOpenException>(() =>
                policy.ExecuteAsync((ctx, t) => 0, CancellationToken.None));

            // wait until the open duration is being expired
            Thread.Sleep(openException.RemainingOpenTime.Add(TimeSpan.FromMilliseconds(10)));

            // simulate fast parallel calls, the faster one will win and attempts to close the circuit, the other one will be rejected
            var tasks = new List<Task>();
            for (var i = 0; i < 2; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await policy.ExecuteAsync(async (ctx, t) =>
                        {
                            counter++;
                            Assert.AreEqual(State.HalfOpen, state);
                            await Task.Delay(TimeSpan.FromMilliseconds(300), t);
                            return 0;
                        }, CancellationToken.None);

                    }
                    catch (Exception e)
                    {
                        Assert.IsInstanceOfType(e, typeof(HalfOpenExecutionLimitExceededException));
                    }
                }));
            }

            await Task.WhenAll(tasks);

            Assert.AreEqual(State.HalfOpen, state);

            Assert.AreEqual(1, counter);

            // next call will close the circuit
            await policy.ExecuteAsync((ctx, t) =>
            {
                counter++;
                Assert.AreEqual(State.HalfOpen, state);
                return 0;
            }, CancellationToken.None);

            Assert.AreEqual(State.Closed, state);

            Assert.AreEqual(2, counter);
        }
    }
}
