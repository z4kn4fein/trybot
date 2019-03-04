using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry;
using Trybot.Retry.Exceptions;

namespace Trybot.Tests.RetryTests
{
    [TestClass]
    public class RetryTestsNoResultAsync
    {
        private IBotPolicy CreatePolicyWithRetry(RetryConfiguration retryConfig) =>
            new BotPolicy(config => config
                .Configure(botconfig => botconfig
                    .Retry(retryConfig)));

        private RetryConfiguration CreateConfiguration(int count) =>
            new RetryConfiguration()
                .WithMaxAttemptCount(count)
                .WhenExceptionOccurs(ex => true);

        [TestMethod]
        public async Task RetryTestsAsync_Action_Ok()
        {
            var onSucceeded = false;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(2)
                .OnRetrySucceeded(ctx => onSucceeded = true));
            var counter = 0;
            await policy.ExecuteAsync((ctx, t) => { counter++; }, CancellationToken.None);

            Assert.AreEqual(1, counter);
            Assert.IsFalse(onSucceeded);
        }

        [TestMethod]
        public async Task RetryTestsAsync_Action_Fail()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(2));
            var counter = 0;

            Action<ExecutionContext, CancellationToken> action = (ctx, t) =>
            {
                counter++;
                throw new Exception();
            };

            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => await policy.ExecuteAsync(action, CancellationToken.None));
            Assert.AreEqual(2, counter);
        }

        [TestMethod]
        public async Task RetryTestsAsync_Func_Fail()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(2));
            var counter = 0;

            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => await policy.ExecuteAsync((ctx, t) =>
            {
                counter++;
                throw new Exception();
            }, CancellationToken.None));
            Assert.AreEqual(2, counter);
        }

        [TestMethod]
        public async Task RetryTestsAsync_Action_Fail_Cancel()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(30));
            var counter = 0;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(1));

            Action<ExecutionContext, CancellationToken> action = (ctx, t) =>
            {
                counter++;
                Task.Delay(TimeSpan.FromMilliseconds(500), t).Wait(t);
                throw new Exception();
            };

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => await policy.ExecuteAsync(action, source.Token));

            Assert.IsTrue(counter < 30 && counter > 0);
        }

        [TestMethod]
        public async Task RetryTests_Action_Fail_Then_Success()
        {
            var onSucceeded = false;
            var onRetryAsync = false;
            var onSucceededAsync = false;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(2)
                .OnRetryAsync((ex, ctx, t) => { onRetryAsync = true; return Task.FromResult(0); })
                .OnRetrySucceeded(ctx => onSucceeded = true)
                .OnRetrySucceededAsync((ctx, t) => { onSucceededAsync = true; return Task.FromResult(0); }));
            var counter = 0;

            Action<ExecutionContext, CancellationToken> action = (ctx, t) =>
            {
                if (counter < 1)
                {
                    counter++;
                    throw new Exception();
                }
            };

            await policy.ExecuteAsync(action, CancellationToken.None);

            Assert.IsTrue(onSucceeded);
            Assert.IsTrue(onRetryAsync);
            Assert.IsTrue(onSucceededAsync);
        }

        [TestMethod]
        public async Task RetryTestsAsync_Func_Fail_Cancel()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(30));
            var counter = 0;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(1));

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => await policy.ExecuteAsync((ctx, t) =>
            {
                counter++;
                Task.Delay(TimeSpan.FromMilliseconds(500), t).Wait(t);
                throw new Exception();
            }, source.Token));

            Assert.IsTrue(counter < 30 && counter > 0);
        }

        [TestMethod]
        public async Task RetryTestsAsync_Action_Fail_Wait()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(5)
                .WaitBetweenAttempts((attempt, ex) => TimeSpan.FromMilliseconds(200)));
            var counter = 0;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromMilliseconds(500));

            Action<ExecutionContext, CancellationToken> action = (ctx, t) =>
            {
                counter++;
                throw new Exception();
            };

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => await policy.ExecuteAsync(action, source.Token));

            Assert.IsTrue(counter >= 2 && counter < 5);
        }

        [TestMethod]
        public async Task RetryTestsAsync_Func_Fail_Wait()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(5)
                .WaitBetweenAttempts((attempt, ex) => TimeSpan.FromMilliseconds(200)));
            var counter = 0;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromMilliseconds(500));

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => await policy.ExecuteAsync((ctx, t) =>
            {
                counter++;
                throw new Exception();
            }, source.Token));

            Assert.IsTrue(counter >= 2 && counter < 5);
        }

        [TestMethod]
        public async Task RetryTestsAsync_Action_Fail_OnRetry()
        {
            var onRetryCounter = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(5).OnRetry((ex, ctx) => onRetryCounter = ctx.CurrentAttempt));
            var counter = 0;

            Action<ExecutionContext, CancellationToken> action = (ctx, t) =>
            {
                counter++;
                throw new Exception();
            };

            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => await policy.ExecuteAsync(action, CancellationToken.None));

            Assert.AreEqual(counter, onRetryCounter);
        }

        [TestMethod]
        public async Task RetryTestsAsync_Func_Fail_OnRetry()
        {
            var onRetryCounter = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(5).OnRetry((ex, ctx) => onRetryCounter = ctx.CurrentAttempt));
            var counter = 0;

            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => await policy.ExecuteAsync((ctx, t) =>
            {
                counter++;
                throw new Exception();
            }, CancellationToken.None));

            Assert.AreEqual(counter, onRetryCounter);
        }

        [TestMethod]
        public async Task RetryTestsAsync_Action_Fail_OnRetryAsync()
        {
            var onRetryCounter = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(5).OnRetryAsync((ex, ctx, t) =>
            {
                onRetryCounter = ctx.CurrentAttempt;
                return Task.FromResult(0);
            }));
            var counter = 0;

            Action<ExecutionContext, CancellationToken> action = (ctx, t) =>
            {
                counter++;
                throw new Exception();
            };

            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => await policy.ExecuteAsync(action, CancellationToken.None));

            Assert.AreEqual(counter, onRetryCounter);
        }

        [TestMethod]
        public async Task RetryTestsAsync_Func_Fail_OnRetryAsync()
        {
            var onRetryCounter = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(5).OnRetryAsync((ex, ctx, t) =>
            {
                onRetryCounter = ctx.CurrentAttempt;
                return Task.FromResult(0);
            }));
            var counter = 0;

            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => await policy.ExecuteAsync((ctx, t) =>
            {
                counter++;
                throw new Exception();
            }, CancellationToken.None));

            Assert.AreEqual(counter, onRetryCounter);
        }

        [TestMethod]
        public async Task RetryTests_Action_Handles_Only_Configured_Exception()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(2)
                .RetryIndefinitely()
                .WhenExceptionOccurs(ex => ex is NullReferenceException));
            var counter = 0;
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                policy.ExecuteAsync((ctx, t) =>
                {
                    counter++;
                    throw new InvalidOperationException();
                }, CancellationToken.None));

            Assert.AreEqual(1, counter);
        }
    }
}
