using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry;
using Trybot.Retry.Exceptions;

namespace Trybot.Tests.RetryTests
{
    [TestClass]
    public class RetryTestsSync
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
        public void RetryTests_Action_Ok()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(2));
            var counter = 0;
            var result = policy.Execute((ctx, t) => { counter++; return 5; }, CancellationToken.None);

            Assert.AreEqual(5, result);
            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void RetryTests_Action_Ok_Config()
        {
            var policy = new BotPolicy<int>(config => config
                .Configure(botconfig => botconfig
                    .Retry(retryConfig => retryConfig.WithMaxAttemptCount(2).WhenExceptionOccurs(ex => true))));
            var counter = 0;
            var result = policy.Execute((ctx, t) => { counter++; return 5; }, CancellationToken.None);

            Assert.AreEqual(5, result);
            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void RetryTests_Action_Fail()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(2));
            var counter = 0;
            var result = 0;
            var exception = new Exception();
            var retryException = Assert.ThrowsException<MaxRetryAttemptsReachedException>(() => result = policy.Execute((ctx, t) =>
            {
                counter++;
                throw exception;
            }, CancellationToken.None));

            Assert.AreEqual(exception, retryException.InnerException);
            Assert.AreEqual(0, result);
            Assert.AreEqual(2, counter);
        }

        [TestMethod]
        public void RetryTests_Action_Fail_Cancel()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(30));
            var counter = 0;
            var result = 0;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(1));

            Assert.ThrowsException<OperationCanceledException>(() => result = policy
                .Execute((ctx, t) =>
                {
                    counter++;
                    Task.Delay(TimeSpan.FromMilliseconds(500), t).Wait(t);
                    throw new Exception();
                }, source.Token));

            Assert.AreEqual(0, result);
            Assert.IsTrue(counter < 30 && counter > 0);
        }

        [TestMethod]
        public void RetryTests_Action_Fail_Wait()
        {
            Exception delayFactoryExceptionParam = null;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(5)
                .WaitBetweenAttempts((attempt, ex) =>
                {
                    delayFactoryExceptionParam = ex;
                    return TimeSpan.FromMilliseconds(200);
                }));
            var counter = 0;
            var result = 0;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromMilliseconds(500));
            var exception = new Exception();
            Assert.ThrowsException<OperationCanceledException>(() => result = policy
                .Execute((ctx, t) =>
                {
                    counter++;
                    throw exception;
                }, source.Token));

            Assert.AreEqual(0, result);
            Assert.AreSame(exception, delayFactoryExceptionParam);
            Assert.IsTrue(counter > 2 && counter < 5);
        }

        [TestMethod]
        public void RetryTests_Action_Fail_Wait_WithResult_OnRetry()
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

            Assert.ThrowsException<OperationCanceledException>(() => result = policy
                .Execute((ctx, t) =>
                {
                    counter++;
                    return 6;
                }, source.Token));

            Assert.AreEqual(0, result);
            Assert.AreEqual(6, delayResult);
            Assert.AreEqual(TimeSpan.FromMilliseconds(200), nextDelay);
            Assert.IsTrue(counter > 2 && counter < 5);
        }

        [TestMethod]
        public void RetryTests_Action_Fail_OnRetry()
        {
            var onRetryCounter = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(5).OnRetry((ex, ctx) => onRetryCounter = ctx.CurrentAttempt));
            var counter = 0;
            var result = 0;

            Assert.ThrowsException<MaxRetryAttemptsReachedException>(() => result = policy
                .Execute((ctx, t) =>
                {
                    counter++;
                    throw new Exception();
                }, CancellationToken.None));

            Assert.AreEqual(0, result);
            Assert.AreEqual(counter, onRetryCounter);
        }

        [TestMethod]
        public void RetryTests_Action_Fail_OnRetry_WithResult()
        {
            var onRetryCounter = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(5).OnRetry((r, ex, ctx) => onRetryCounter = ctx.CurrentAttempt));
            var counter = 0;
            var result = 0;

            Assert.ThrowsException<MaxRetryAttemptsReachedException>(() => result = policy
                .Execute((ctx, t) =>
                {
                    counter++;
                    throw new Exception();
                }, CancellationToken.None));

            Assert.AreEqual(0, result);
            Assert.AreEqual(counter, onRetryCounter);
        }

        [TestMethod]
        public void RetryTests_Action_Fail_ResultFilter()
        {
            var onRetryCounter = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(5)
                .WhenResultIs(r => r != 5)
                .OnRetry((ex, ctx) => onRetryCounter = ctx.CurrentAttempt));
            var counter = 0;
            var result = 0;

            var exception = Assert.ThrowsException<MaxRetryAttemptsReachedException>(() => result = policy
                .Execute((ctx, t) => { counter++; return 6; }, CancellationToken.None));

            Assert.AreEqual(0, result);
            Assert.AreEqual(6, exception.OperationResult);
            Assert.AreEqual(counter, onRetryCounter);
        }

        [TestMethod]
        public void RetryTests_Action_Fail_ResultFilter_OnRetryResult()
        {
            var onRetryCounter = 0;
            var handlerResult = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(5)
                .WhenResultIs(r => r != 5)
                .OnRetry((r, ex, ctx) => { onRetryCounter = ctx.CurrentAttempt; handlerResult = r; }));
            var counter = 0;
            var result = 0;

            Assert.ThrowsException<MaxRetryAttemptsReachedException>(() => result = policy
                .Execute((ctx, t) => { counter++; return 6; }, CancellationToken.None));

            Assert.AreEqual(0, result);
            Assert.AreEqual(6, handlerResult);
            Assert.AreEqual(counter, onRetryCounter);
        }

        [TestMethod]
        public void RetryTests_Action_Handles_Only_Configured_Exception()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration<int>(2)
                .RetryIndefinitely()
                .WhenExceptionOccurs(ex => ex is InvalidOperationException));
            var counter = 0;
            Assert.ThrowsException<NullReferenceException>(() =>
                policy.Execute((ctx, t) =>
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
