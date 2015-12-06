using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Tests
{
    public partial class RetryManagerTests
    {
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExecuteAsync_FuncTask_WithoutFilter_Exception()
        {
            this.retryManager.ExecuteAsync(async () =>
            {
                await Task.Run(() =>
                {
                    throw new Exception();
                });
            }, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExecuteAsync_FuncTask_WithoutFilter_Exception_WithoutCancellationToken()
        {
            this.retryManager.ExecuteAsync(async () =>
            {
                await Task.Run(() =>
                {
                    throw new Exception();
                });
            }, (attempt, nextDelay) => { }, this.executionPolicy).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExecuteAsync_FuncTask_WithoutFilter_CancellationToken_Exception()
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
            task.Wait();
        }

        [TestMethod]
        public void ExecuteAsync_FuncTask_WithoutFilter()
        {
            try
            {
                this.retryManager.ExecuteAsync(async () =>
                {
                    await Task.Run(() =>
                    {
                        throw new Exception();
                    });
                }, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy).Wait();
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.Counter);
            }
        }

        [TestMethod]
        public void ExecuteAsync_FuncTask_WithFilter_False()
        {
            try
            {
                this.retryManager.ExecuteAsync(async () =>
                {
                    await Task.Run(() =>
                    {
                        throw new Exception();
                    });
                }, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => false).Wait();
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.Counter);
            }
        }

        [TestMethod]
        public void ExecuteAsync_FuncTask_WithoutFilter_CancellationToken()
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
                task.Wait();
            }
            catch (Exception)
            {

                Assert.IsTrue(this.executionPolicy.Counter < 5);
            }
        }

        [TestMethod]
        public void ExecuteAsync_FuncTask_WithFilter_True_CancellationToken()
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
                task.Wait();
            }
            catch (Exception)
            {

                Assert.AreEqual(0, this.executionPolicy.Counter);
            }
        }

        [TestMethod]
        public void ExecuteAsync_FuncTask_WithFilter_False_CancellationToken()
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
                task.Wait();
            }
            catch (Exception)
            {

                Assert.IsTrue(this.executionPolicy.Counter < 5);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExecuteAsync_FuncTask_WithFilter_True_Exception()
        {
            this.retryManager.ExecuteAsync(async () =>
            {
                await Task.Run(() =>
                {
                    throw new Exception();
                });
            }, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => true).Wait();
        }

        [TestMethod]
        public void ExecuteAsync_FuncTask_WithFilter_True()
        {
            try
            {
                this.retryManager.ExecuteAsync(async () =>
                {
                    await Task.Run(() =>
                    {
                        throw new Exception();
                    });
                }, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => true).Wait();
            }
            catch (Exception)
            {
                Assert.AreEqual(0, this.executionPolicy.Counter);
            }
        }

        [TestMethod]
        public void ExecuteAsync_FuncTask_WithoutRetry()
        {
            this.retryManager.ExecuteAsync(() => Task.Run(() => { }), CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy).Wait();
            Assert.AreEqual(0, this.executionPolicy.Counter);
        }

        [TestMethod]
        public void ExecuteAsync_FuncTask_WithoutRetry_WithoutCancellationToken()
        {
            this.retryManager.ExecuteAsync(() => Task.Run(() => { }), (attempt, nextDelay) => { }, this.executionPolicy).Wait();
            Assert.AreEqual(0, this.executionPolicy.Counter);
        }
    }
}
