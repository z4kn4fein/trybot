using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Tests
{
    public partial class RetryManagerTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTaskWithResult_WithoutFilter_Exception()
        {
            await this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetException(new Exception());
                return taskCompletionSource.Task;
            }, null, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTaskWithResult_Exception()
        {
            var rm = new RetryManager();
            await rm.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetException(new Exception());
                return taskCompletionSource.Task;
            }, null, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTaskWithResult_WithoutFilter_ExplicitPolicy()
        {
            await new RetryManager().ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetException(new Exception());
                return taskCompletionSource.Task;
            }, exp => true, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTaskWithResult_WithoutFilter_Exception_WithoutCancellationToken()
        {
            await this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetException(new Exception());
                return taskCompletionSource.Task;
            }, onRetryOccured: (attempt, nextDelay) => { }, retryStartegy: this.executionPolicy);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTaskWithResult_WithoutFilter_CancellationToken_Exception()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var task = this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetException(new Exception());
                return taskCompletionSource.Task;
            }, null, cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy);
            cancellationTokenSource.Cancel();
            await task;
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTaskWithResult_WithoutFilter()
        {
            try
            {
                await this.retryManager.ExecuteAsync(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetException(new Exception());
                    return taskCompletionSource.Task;
                }, null, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTaskWithResult_WithFilter_False()
        {
            try
            {
                await this.retryManager.ExecuteAsync(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetException(new Exception());
                    return taskCompletionSource.Task;
                }, null, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => false);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTaskWithResult_WithoutFilter_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetException(new Exception());
                    return taskCompletionSource.Task;
                }, null, cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy);
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
        public async Task ExecuteAsync_FuncTaskWithResult_WithFilter_True_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetException(new Exception());
                    return taskCompletionSource.Task;
                }, null, cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy, () => true);
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
        public async Task ExecuteAsync_FuncTaskWithResult_WithFilter_False_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetException(new Exception());
                    return taskCompletionSource.Task;
                }, null, cancellationTokenSource.Token, (attempt, nextDelay) => { }, this.executionPolicy, () => false);
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
        public async Task ExecuteAsync_FuncTaskWithResult_WithFilter_True_Exception()
        {
            await this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetException(new Exception());
                return taskCompletionSource.Task;
            }, null, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => true);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ExecuteAsync_FuncTaskWithResult_WithFilter_True()
        {
            try
            {
                await this.retryManager.ExecuteAsync(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetException(new Exception());
                    return taskCompletionSource.Task;
                }, null, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, () => true);
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
                throw;
            }
        }

        [TestMethod]
        public async Task ExecuteAsync_FuncTaskWithResult_WithoutRetry()
        {
            await this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetResult(null);
                return taskCompletionSource.Task;
            }, null, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy);
            Assert.AreEqual(0, this.executionPolicy.CurrentAttempt);
        }

        [TestMethod]
        public async Task ExecuteAsync_FuncTaskWithResult_WithResultFilter()
        {
            var functionResult = await this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.SetResult(true);
                return taskCompletionSource.Task;
            }, null, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, resultFilter: result => result);
            Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
            Assert.IsTrue(functionResult);
        }

        [TestMethod]
        public async Task ExecuteAsync_FuncTaskWithResult_WithResultFilter_WithRetryFilter()
        {
            var functionResult = await this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.SetResult(false);
                return taskCompletionSource.Task;
            }, null, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, resultFilter: result => result, retryFiler: () => true);
            Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
            Assert.IsFalse(functionResult);
        }

        [TestMethod]
        public async Task ExecuteAsync_FuncTaskWithResult_WithResultFilter_WithoutCancellationToken()
        {
            var functionResult = await this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.SetResult(true);
                return taskCompletionSource.Task;
            }, onRetryOccured: (attempt, nextDelay) => { }, retryStartegy: this.executionPolicy, resultFilter: result => result);
            Assert.AreEqual(5, this.executionPolicy.CurrentAttempt);
            Assert.IsTrue(functionResult);
        }

        [TestMethod]
        public async Task ExecuteAsync_FuncTaskWithResult_WithResultFilter_False()
        {
            var functionResult = await this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.SetResult(true);
                return taskCompletionSource.Task;
            }, null, CancellationToken.None, (attempt, nextDelay) => { }, this.executionPolicy, resultFilter: result => !result);
            Assert.AreEqual(0, this.executionPolicy.CurrentAttempt);
            Assert.IsTrue(functionResult);
        }

        [TestMethod]
        public async Task ExecuteAsync_FuncTaskWithResult_WithResultFilter_False_WithoutCancellationToken()
        {
            var functionResult = await this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.SetResult(true);
                return taskCompletionSource.Task;
            }, onRetryOccured: (attempt, nextDelay) => { }, retryStartegy: this.executionPolicy, resultFilter: result => !result);
            Assert.AreEqual(0, this.executionPolicy.CurrentAttempt);
            Assert.IsTrue(functionResult);
        }
    }
}
