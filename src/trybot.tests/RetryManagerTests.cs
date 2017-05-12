using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Interfaces;
using Trybot.Strategy;

namespace Trybot.Tests
{
    [TestClass]
    public partial class RetryManagerTests
    {
        private IRetryManager retryManager;
        private RetryStartegy executionPolicy;

        [TestInitialize]
        public void Init()
        {
            this.executionPolicy = new FixedIntervalRetryStartegy(5, TimeSpan.FromMilliseconds(5));
            var mockRetryPolicy = new Mock<IRetryPolicy>();
            mockRetryPolicy.Setup(policy => policy.ShouldRetryAfter(It.IsAny<Exception>())).Returns(true);
            this.retryManager = new RetryManager(mockRetryPolicy.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_Action_WithoutFilter_Exception()
        {
            await this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_Action_WithoutFilter_WithoutCancellationToken_Exception()
        {
            await this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), (attempt, nextDelay) => { }, this.executionPolicy);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_Action_WithoutFilter_CancellationToken_Exception()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var task = this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy);
            cancellationTokenSource.Cancel();
            await task;
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_Action_WithoutFilter()
        {
            try
            {
                await this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_Action_WithFilter_False()
        {
            try
            {
                await this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => false);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_Action_WithoutFilter_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), cancellationTokenSource.Token);
                cancellationTokenSource.Cancel();
                await task;
            }
            catch (Exception)
            {
                Assert.IsTrue(this.executionPolicy.CurrentAttempt < 5);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_Action_WithFilter_True_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy, () => true);
                cancellationTokenSource.Cancel();
                await task;
            }
            catch (Exception)
            {
                Assert.IsTrue(this.executionPolicy.CurrentAttempt < 5);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_Action_WithFilter_False_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy, () => false);
                cancellationTokenSource.Cancel();
                await task;
            }
            catch (Exception)
            {

                Assert.IsTrue(this.executionPolicy.CurrentAttempt < 5);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_Action_WithFilter_True_Exception()
        {
            await this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => true);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_Action_WithFilter_True()
        {
            try
            {
                await this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => true);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
                throw;
            }
        }

        [TestMethod]
        public async Task ExecuteAsync_Action_WithoutRetry()
        {
            await this.retryManager.ExecuteAsync(() => { }, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy);
            Assert.AreEqual(0, this.executionPolicy.CurrentAttempt);
        }

        [TestMethod]
        public async Task ExecuteAsync_Action_WithFilter_NoException()
        {
            await this.retryManager.ExecuteAsync(() => { }, CancellationToken.None, (attempt, nextDelay) => { },
                this.executionPolicy, () => true);
            Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
        }

        [TestMethod]
        public async Task ExecuteAsync_Action_WithoutRetry_WithoutCancellationToken()
        {
            await this.retryManager.ExecuteAsync(() => { }, (attempt, nextDelay) => { }, this.executionPolicy);
            Assert.AreEqual(0, this.executionPolicy.CurrentAttempt);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_Action_WithoutFilter_WithRetryOccuredEvent()
        {
            try
            {
                await this.retryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) =>
                {
                    Assert.IsTrue(attempt > 0);
                    Assert.AreEqual(TimeSpan.FromMilliseconds(5), nextDelay);
                }, this.executionPolicy);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_Action_ForceThrowException()
        {
            try
            {
                var retryPolicy = new Mock<IRetryPolicy>();
                retryPolicy.Setup(policy => policy.ShouldRetryAfter(It.IsAny<Exception>())).Returns(false);
                var localRetryManager = new RetryManager(retryPolicy.Object);
                await localRetryManager.ExecuteAsync((Action)(() => { throw new Exception(); }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy);
            }
            catch (Exception)
            {
                Assert.AreEqual(0, this.executionPolicy.CurrentAttempt);
                throw;
            }
        }
    }
}
