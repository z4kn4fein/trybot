using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using Trybot.Interfaces;
using Trybot.Strategy;

namespace Trybot.Tests
{
    [TestClass]
    public partial class RetryManagerTests
    {
        public class FakeRetryStrategy : RetryStartegy
        {
            public FakeRetryStrategy()
                : base(5, TimeSpan.FromMilliseconds(5))
            { }

            protected override TimeSpan GetNextDelay(int counter)
            {
                return base.Delay;
            }
        }

        public class FakeRetryPolicy : IRetryPolicy
        {
            public bool ShouldRetryAfter(Exception exception)
            {
                return true;
            }
        }

        private IRetryManager retryManager;
        private FakeRetryStrategy executionPolicy;

        [TestInitialize]
        public void Init()
        {
            this.executionPolicy = new FakeRetryStrategy();
            this.retryManager = new RetryManager(new FakeRetryPolicy());
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExecuteAsync_Action_WithoutFilter_Exception()
        {
            this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy).Wait();
            this.retryManager.ExecuteAsync(() =>
            {

            });
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExecuteAsync_Action_WithoutFilter_WithoutCancellationToken_Exception()
        {
            this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), (attempt, nextDelay) => { }, this.executionPolicy).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExecuteAsync_Action_WithoutFilter_CancellationToken_Exception()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var task = this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy);
            cancellationTokenSource.Cancel();
            task.Wait();
        }

        [TestMethod]
        public void ExecuteAsync_Action_WithoutFilter()
        {
            try
            {
                this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy).Wait();
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
            }
        }

        [TestMethod]
        public void ExecuteAsync_Action_WithFilter_False()
        {
            try
            {
                this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => false).Wait();
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
            }
        }

        [TestMethod]
        public void ExecuteAsync_Action_WithoutFilter_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), cancellationTokenSource.Token);
                cancellationTokenSource.Cancel();
                task.Wait();
            }
            catch (Exception)
            {

                Assert.IsTrue(this.executionPolicy.CurrentAttempt < 5);
            }
        }

        [TestMethod]
        public void ExecuteAsync_Action_WithFilter_True_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy, () => true);
                cancellationTokenSource.Cancel();
                task.Wait();
            }
            catch (Exception)
            {

                Assert.AreEqual(0, this.executionPolicy.CurrentAttempt);
            }
        }

        [TestMethod]
        public void ExecuteAsync_Action_WithFilter_False_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy, () => false);
                cancellationTokenSource.Cancel();
                task.Wait();
            }
            catch (Exception)
            {

                Assert.IsTrue(this.executionPolicy.CurrentAttempt < 5);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExecuteAsync_Action_WithFilter_True_Exception()
        {
            this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => true).Wait();
        }

        [TestMethod]
        public void ExecuteAsync_Action_WithFilter_True()
        {
            try
            {
                this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => true).Wait();
            }
            catch (Exception)
            {
                Assert.AreEqual(0, this.executionPolicy.CurrentAttempt);
            }
        }

        [TestMethod]
        public void ExecuteAsync_Action_WithoutRetry()
        {
            this.retryManager.ExecuteAsync(() => { }, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy).Wait();
            Assert.AreEqual(0, this.executionPolicy.CurrentAttempt);
        }

        [TestMethod]
        public void ExecuteAsync_Action_WithoutRetry_WithoutCancellationToken()
        {
            this.retryManager.ExecuteAsync(() => { }, (attempt, nextDelay) => { }, this.executionPolicy).Wait();
            Assert.AreEqual(0, this.executionPolicy.CurrentAttempt);
        }

        [TestMethod]
        public void ExecuteAsync_Action_WithoutFilter_WithRetryOccuredEvent()
        {
            try
            {
                this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) =>
                {
                    Console.WriteLine($"{attempt} {nextDelay.TotalSeconds}");
                    Assert.IsTrue(attempt > 0);
                    Assert.AreEqual(TimeSpan.FromMilliseconds(5), nextDelay);
                }, this.executionPolicy).Wait();
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
            }
        }
    }
}
