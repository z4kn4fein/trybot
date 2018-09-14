using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry;
using Trybot.Retry.Exceptions;

namespace Trybot.Tests.RetryTests
{
    [TestClass]
    public class RetryTestsNoResultSync
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
        public void RetryTests_Action_Ok()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(2));
            var counter = 0;
            policy.Execute((ctx, t) => { counter++; }, CancellationToken.None);

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void RetryTests_Action_Handles_Only_Configured_Exception()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(2)
                .WhenExceptionOccurs(ex => ex is NullReferenceException));
            var counter = 0;
            Assert.ThrowsException<InvalidOperationException>(() =>
                policy.Execute((ctx, t) =>
                {
                    counter++;
                    throw new InvalidOperationException();
                }, CancellationToken.None));

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void RetryTests_Action_Fail()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(2));
            var counter = 0;

            Assert.ThrowsException<MaxRetryAttemptsReachedException>(() => policy.Execute((ctx, t) =>
                {
                    counter++;
                    throw new Exception();
                }, CancellationToken.None));
            Assert.AreEqual(2, counter);
        }

        [TestMethod]
        public void RetryTests_Action_Fail_Cancel()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(30));
            var counter = 0;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(1));

            Assert.ThrowsException<OperationCanceledException>(() => policy
                .Execute((ctx, t) =>
                {
                    counter++;
                    Task.Delay(TimeSpan.FromMilliseconds(500), t).Wait(t);
                    throw new Exception();
                }, source.Token));

            Assert.IsTrue(counter < 30 && counter > 0);
        }

        [TestMethod]
        public void RetryTests_Action_Fail_Wait()
        {
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(5)
                .WaitBetweenAttempts((attempt, ex) => TimeSpan.FromMilliseconds(200)));
            var counter = 0;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromMilliseconds(500));

            Assert.ThrowsException<OperationCanceledException>(() => policy
                .Execute((ctx, t) =>
                {
                    counter++;
                    throw new Exception();
                }, source.Token));

            Assert.IsTrue(counter > 2 && counter < 5);
        }

        [TestMethod]
        public void RetryTests_Action_Fail_OnRetry()
        {
            var onRetryCounter = 0;
            var policy = this.CreatePolicyWithRetry(this.CreateConfiguration(5).OnRetry((ex, ctx) => onRetryCounter = ctx.CurrentAttempt));
            var counter = 0;

            Assert.ThrowsException<MaxRetryAttemptsReachedException>(() => policy
                .Execute((ctx, t) =>
                {
                    counter++;
                    throw new Exception();
                }, CancellationToken.None));

            Assert.AreEqual(counter, onRetryCounter);
        }
    }
}
