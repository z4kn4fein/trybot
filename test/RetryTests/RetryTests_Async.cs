using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry;
using Trybot.Retry.Exceptions;

namespace Trybot.Tests.RetryTests
{
    [TestClass]
    public class RetryTestsAsync
    {
        private IBotPolicy<T> CreatePolicyWithRetry<T>(RetryConfiguration<T> retryConfig) =>
            new BotPolicy<T>(config => config
                .Configure(botconfig => botconfig
                    .Retry(retryConfig)));

        private RetryConfiguration<T> CreateConfiguration<T>(int count) =>
            new RetryConfiguration<T>()
                .WithMaxAttemptCount(count)
                .WhenExceptionOccurs(ex => true);

        [TestMethod]
        public async Task RetryTests_Func_Ok()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(2));
            var counter = 0;
            var result = await policy.ExecuteAsync((ctx, t) => { counter++; return 5; }, CancellationToken.None);

            Assert.AreEqual(5, result);
            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public async Task RetryTests_Func_Task_Ok()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(2));
            var counter = 0;
            var result = await policy.ExecuteAsync((ctx, t) => { counter++; return Task.FromResult(5); }, CancellationToken.None);

            Assert.AreEqual(5, result);
            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public async Task RetryTests_Func_Task_Fail()
        {
            var onRetry = 0;
            var onRetryAsync = 0;
            var onRetryResult = 0;
            var onRetryResultAsync = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(5)
                .OnRetryAsync((ex, att, t) => { onRetryAsync = att.CurrentAttempt; return Task.FromResult(0); })
                .OnRetryAsync((r, ex, att, t) => { onRetryResultAsync = att.CurrentAttempt; return Task.FromResult(0); })
                .OnRetry((ex, att) => onRetry = att.CurrentAttempt)
                .OnRetry((r, ex, att) => onRetryResult = att.CurrentAttempt));
            var counter = 0;
            var result = 0;
            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => result = await policy.ExecuteAsync((ctx, t) =>
            {
                counter++;
                object a = null;
                a.GetHashCode();
                return Task.FromResult(5);
            }, CancellationToken.None));

            Assert.AreEqual(0, result);
            Assert.AreEqual(5, counter);
            Assert.AreEqual(counter, onRetry);
            Assert.AreEqual(counter, onRetryAsync);
            Assert.AreEqual(counter, onRetryResult);
            Assert.AreEqual(counter, onRetryResultAsync);
        }

        [TestMethod]
        public async Task RetryTests_Func_Task_Result_Fail()
        {
            var onRetryResult = 0;
            var onRetryResultAsync = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(2)
                .OnRetryAsync((r, ex, att, t) => { onRetryResultAsync = r; return Task.FromResult(0); })
                .OnRetry((r, ex, att) => onRetryResult = r)
                .WhenResultIs(r => r != 6));
            var counter = 0;
            var result = 0;
            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => result = await policy.ExecuteAsync((ctx, t) =>
            {
                counter++;
                return Task.FromResult(5);
            }, CancellationToken.None));

            Assert.AreEqual(0, result);
            Assert.AreEqual(2, counter);
            Assert.AreEqual(5, onRetryResult);
            Assert.AreEqual(5, onRetryResultAsync);
        }

        [TestMethod]
        public async Task RetryTests_Func_Fail()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(2));
            var counter = 0;
            var result = 0;

            Func<ExecutionContext, CancellationToken, int> operation = (ctx, t) =>
            {
                counter++;
                throw new Exception();
            };

            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => result = await policy.ExecuteAsync(operation, CancellationToken.None));

            Assert.AreEqual(0, result);
            Assert.AreEqual(2, counter);
        }

        [TestMethod]
        public async Task RetryTests_Func_Fail_Cancel()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(30));
            var counter = 0;
            var result = 0;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(1));

            Func<ExecutionContext, CancellationToken, int> operation = (ctx, t) =>
            {
                counter++;
                Task.Delay(TimeSpan.FromMilliseconds(500), t).Wait(t);
                throw new Exception();
            };

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => result = await policy
                .ExecuteAsync(operation, source.Token));

            Assert.AreEqual(0, result);
            Assert.IsTrue(counter < 30 && counter > 0);
        }

        [TestMethod]
        public async Task RetryTests_Func_Fail_Wait()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(5)
                .WaitBetweenAttempts((attempt, ex) => TimeSpan.FromMilliseconds(200)));
            var counter = 0;
            var result = 0;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromMilliseconds(500));

            Func<ExecutionContext, CancellationToken, int> operation = (ctx, t) =>
            {
                counter++;
                throw new Exception();
            };

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => result = await policy
                .ExecuteAsync(operation, source.Token));

            Assert.AreEqual(0, result);
            Assert.IsTrue(counter >= 2 && counter < 5);
        }

        [TestMethod]
        public async Task RetryTests_Func_Fail_Wait_WithResult_OnRetry()
        {
            var delayResult = 0;
            var nextDelay = TimeSpan.Zero;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(5).WaitBetweenAttempts((attempt, ex, r) =>
            {
                delayResult = r;
                return TimeSpan.FromMilliseconds(200);
            })
            .WhenResultIs(r => r != 5)
            .OnRetry((ex, ctx) => nextDelay = ctx.CurrentDelay));
            var counter = 0;
            var result = 0;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromMilliseconds(500));

            Func<ExecutionContext, CancellationToken, int> operation = (ctx, t) =>
            {
                counter++;
                return 6;
            };

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => result = await policy
                .ExecuteAsync(operation, source.Token));

            Assert.AreEqual(0, result);
            Assert.AreEqual(6, delayResult);
            Assert.AreEqual(TimeSpan.FromMilliseconds(200), nextDelay);
            Assert.IsTrue(counter >= 2 && counter < 5);
        }

        [TestMethod]
        public async Task RetryTests_Func_Fail_OnRetry()
        {
            var onRetryCounter = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(5).OnRetry((ex, ctx) => onRetryCounter = ctx.CurrentAttempt));
            var counter = 0;
            var result = 0;

            Func<ExecutionContext, CancellationToken, int> operation = (ctx, t) =>
            {
                counter++;
                throw new Exception();
            };

            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => result = await policy
                .ExecuteAsync(operation, CancellationToken.None));

            Assert.AreEqual(0, result);
            Assert.AreEqual(counter, onRetryCounter);
        }

        [TestMethod]
        public async Task RetryTests_Func_Fail_OnRetry_WithResult()
        {
            var onRetryCounter = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(5).OnRetry((r, ex, ctx) => onRetryCounter = ctx.CurrentAttempt));
            var counter = 0;
            var result = 0;

            Func<ExecutionContext, CancellationToken, int> operation = (ctx, t) =>
            {
                counter++;
                throw new Exception();
            };

            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => result = await policy
                .ExecuteAsync(operation, CancellationToken.None));

            Assert.AreEqual(0, result);
            Assert.AreEqual(counter, onRetryCounter);
        }

        [TestMethod]
        public async Task RetryTests_Func_Fail_ResultFilter()
        {
            var onRetryCounter = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(5)
                .WhenResultIs(r => r != 5)
                .OnRetry((ex, ctx) => onRetryCounter = ctx.CurrentAttempt));
            var counter = 0;
            var result = 0;

            Func<ExecutionContext, CancellationToken, int> operation = (ctx, t) =>
                { counter++; return 6; };

            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => result = await policy
                .ExecuteAsync(operation, CancellationToken.None));

            Assert.AreEqual(0, result);
            Assert.AreEqual(counter, onRetryCounter);
        }

        [TestMethod]
        public async Task RetryTests_Func_Fail_ResultFilter_OnRetryResult()
        {
            var onRetryCounter = 0;
            var handlerResult = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(5)
                .WhenResultIs(r => r != 5)
                .OnRetry((r, ex, ctx) => { onRetryCounter = ctx.CurrentAttempt; handlerResult = r; }));
            var counter = 0;
            var result = 0;

            Func<ExecutionContext, CancellationToken, int> operation = (ctx, t) =>
                { counter++; return 6; };

            await Assert.ThrowsExceptionAsync<MaxRetryAttemptsReachedException>(async () => result = await policy
                .ExecuteAsync(operation, CancellationToken.None));

            Assert.AreEqual(0, result);
            Assert.AreEqual(6, handlerResult);
            Assert.AreEqual(counter, onRetryCounter);
        }

        [TestMethod]
        public async Task RetryTests_Action_Handles_Only_Configured_Exception()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(2)
                .RetryIndefinitely()
                .WhenExceptionOccurs(ex => ex is InvalidOperationException));
            var counter = 0;
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() =>
                policy.ExecuteAsync((ctx, t) =>
                {
                    counter++;
                    object o = null;
                    o.GetHashCode();
                    return 0;
                }, CancellationToken.None));

            Assert.AreEqual(1, counter);
        }
    }
}
