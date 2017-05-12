using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Strategy;

namespace Trybot.Tests
{
    public partial class RetryManagerTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTask_WithoutFilter_Exception()
        {
            await this.retryManager.ExecuteAsync(async () =>
            {
                await Task.Run(() =>
                {
                    throw new Exception();
                });
            }, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTask_WithoutFilter_Exception_WithoutCancellationToken()
        {
            await this.retryManager.ExecuteAsync(async () =>
            {
                await Task.Run(() =>
                {
                    throw new Exception();
                });
            }, onRetryOccured: (attempt, nextDelay) => { }, retryStartegy: this.executionPolicy);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTask_WithoutFilter_CancellationToken_Exception()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var task = this.retryManager.ExecuteAsync(async () =>
            {
                await Task.Run(() =>
                {
                    throw new Exception();
                });
            }, cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy);
            cancellationTokenSource.Cancel();
            await task;
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTask_WithoutFilter()
        {
            try
            {
                await this.retryManager.ExecuteAsync(async () =>
                {
                    await Task.Run(() =>
                    {
                        throw new Exception();
                    });
                }, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTask_WithFilter_False()
        {
            try
            {
                await this.retryManager.ExecuteAsync(async () =>
                {
                    await Task.Run(() =>
                    {
                        throw new Exception();
                    });
                }, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => false);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTask_WithoutFilter_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync(async () =>
                {
                    await Task.Run(() =>
                    {
                        throw new Exception();
                    });
                }, cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy);
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
        public async Task ExecuteAsync_FuncTask_WithFilter_True_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync(async () =>
                {
                    await Task.Run(() =>
                    {
                        throw new Exception();
                    });
                }, cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy, () => true);
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
        public async Task ExecuteAsync_FuncTask_WithFilter_False_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync(async () =>
                {
                    await Task.Run(() =>
                    {
                        throw new Exception();
                    });
                }, cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy, () => false);
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
        public async Task ExecuteAsync_FuncTask_WithFilter_True_Exception()
        {
            await this.retryManager.ExecuteAsync(async () =>
            {
                await Task.Run(() =>
                {
                    throw new Exception();
                });
            }, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => true);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTask_WithFilter_True()
        {
            try
            {
                await this.retryManager.ExecuteAsync(async () =>
                 {
                     await Task.Run(() =>
                     {
                         throw new Exception();
                     });
                 }, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => true);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
                throw;
            }
        }

        [TestMethod]
        public async Task ExecuteAsync_FuncTask_WithoutRetry()
        {
            await this.retryManager.ExecuteAsync(() => Task.Run(() => { }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy);
            Assert.AreEqual(0, this.executionPolicy.CurrentAttempt);
        }

        [TestMethod]
        public async Task ExecuteAsync_FuncTask_WithFilter_NoException()
        {
            await this.retryManager.ExecuteAsync(() => Task.Run(() => { }), CancellationToken.None, (attempt, nextDelay) => { },
                this.executionPolicy, () => true);
            Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
        }

        [TestMethod]
        public async Task ExecuteAsync_FuncTask_WithoutRetry_WithoutCancellationToken()
        {
            await this.retryManager.ExecuteAsync(() => Task.Run(() => { }), onRetryOccured: (attempt, nextDelay) => { }, retryStartegy: this.executionPolicy);
            Assert.AreEqual(0, this.executionPolicy.CurrentAttempt);
        }

        [TestMethod]
        public async Task ExecuteAsync_FuncTask_WithFilter_NoException_WithoutCancellationToken()
        {
            await this.retryManager.ExecuteAsync(() => Task.Run(() => { }), onRetryOccured: (attempt, nextDelay) => { },
                retryStartegy: this.executionPolicy, retryFiler: () => true);
            Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTask_Load()
        {
            var strategy = new FixedIntervalRetryStartegy(500, TimeSpan.FromMilliseconds(0.5));
            await this.retryManager.ExecuteAsync(async () => await Task.Run(() => { throw new Exception(); }), retryStartegy: strategy);

            Assert.AreEqual(500, strategy.CurrentAttempt);
        }
    }
}
