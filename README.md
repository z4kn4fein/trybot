# trybot [![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/trybot/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/trybot/branch/master) [![Travis CI build status](https://img.shields.io/travis/z4kn4fein/trybot/master.svg?label=travis-ci)](https://travis-ci.org/z4kn4fein/trybot) [![Tests](https://img.shields.io/appveyor/tests/pcsajtai/trybot-1453m/master.svg)](https://ci.appveyor.com/project/pcsajtai/trybot-1453m/build/tests) [![Coverage Status](https://img.shields.io/codecov/c/github/z4kn4fein/trybot.svg)](https://codecov.io/gh/z4kn4fein/trybot) [![Join the chat at https://gitter.im/z4kn4fein/stashbox](https://img.shields.io/gitter/room/z4kn4fein/trybot.svg)](https://gitter.im/z4kn4fein/trybot?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Slack](https://img.shields.io/badge/chat-on%20slack-orange.svg?style=flat)](https://pcsajtai-dev-slack-in.herokuapp.com/)

Trybot is a transient fault handling framework including such built-in bots as [Retry](#retry), [Timeout](#timeout), [Fallback](#fallback) and [Circuit Breaker](#circuit-breaker). The framework is extendable with [custom, user-defined](#custom-bots) bots as well.

Github (stable) | NuGet (stable) | MyGet (pre-release) 
--- | --- | ---
[![Github release](https://img.shields.io/github/release/z4kn4fein/trybot.svg)](https://github.com/z4kn4fein/trybot/releases) | [![NuGet Version](https://buildstats.info/nuget/trybot)](https://www.nuget.org/packages/trybot/) | [![MyGet package](https://img.shields.io/myget/pcsajtai/v/trybot.svg?label=myget)](https://www.myget.org/feed/pcsajtai/package/nuget/trybot)

## Supported platforms

 - .NET 4.5 and above
 - Windows 8/8.1/10
 - Windows Phone Silverlight 8/8.1
 - Windows Phone 8.1
 - Xamarin (Android/iOS/iOS Classic)
 - .NET Standard 1.0/2.0

## Retry

Allows to configure auto re-execution of a given operation based on exceptions thrown, or return values.

### Configuration
    
- For operations **without** a return value
    ```c#
    // Create a new bot policy
    var policy = new BotPolicy();

    // Configure the policy to retry failed operations
    policy.Configure(policyConfig => policyConfig
        .Retry(retryConfig => retryConfig
            // Set the maximum retry count
            .WithMaxAttemptCount(5)
            // Set the predicate which will used to decide that whether an 
            // exception should be handled or not
            .WhenExceptionOccurs(exception => exception is HttpRequestException)
            // Set the delegate function used to calculate the amount of time to 
            // wait between the retry attempts
            .WaitBetweenAttempts((attempt, exception) => TimeSpan.FromSeconds(5))
            // (optional) Set a callback delegate which will be invoked when the 
            // given operation is being re-executed
            .OnRetry((exception, attemptContext) => Console.WriteLine($"{attemptContext.CurrentAttempt}. retry attempt, waiting {attemptContext.CurrentDelay}"))));
    ```
- For operations **with** a return value
    ```c#
    // Create a new bot policy
    var policy = new BotPolicy<HttpResponseMessage>();

    // Configure the policy to retry failed operations
    policy.Configure(policyConfig => policyConfig
        .Retry(retryConfig => retryConfig
            // Set the maximum retry count
            .WithMaxAttemptCount(5)
            // Additionally you can set a predicate which will used to decide that 
            // whether the result of the operation should be handled or not
            .WhenResultIs(result => result.StatusCode != HttpStatusCode.Ok)
            // Set the delegate function used to calculate the amount of time to wait 
            // between the retry attempts
            .WaitBetweenAttempts((attempt, result, exception) => TimeSpan.FromSeconds(5))
            // (optional) Set a callback delegate which will be invoked when the 
            // given operation is being re-executed
            .OnRetry((exception, result, attemptContext) => Console.WriteLine($"{attemptContext.CurrentAttempt}. retry attempt, waiting {attemptContext.CurrentDelay}, result was: {result}"))));
    ```
    
### Available configuration options
- Same for policies **with or without** a return value
    - `.WithMaxAttemptCount(int)` - Sets the maximum number of retry attempts.
    - `.RetryIndefinitely()` - Sets the maximum number of retry attempts to int.MaxValue.
    - `.WaitBetweenAttempts(Func<int, Exception, TimeSpan>)` - Sets the delegate which will be used to calculate the wait time between the retry attempts.
    - `.WhenExceptionOccurs(Func<Exception, bool>)` - Sets the delegate which will be used to determine whether the given operation should be re-executed or not when a specific exception occurs.
    - `.OnRetry(Action<Exception, AttemptContext>)` - Sets the delegate which will be invoked when the given operation is being re-executed.
    - `.OnRetryAsync(Func<Exception, AttemptContext, CancellationToken, Task>)` - Sets the delegate which will be invoked asynchronously when the given operation is being re-executed.
  
- Only for policies **with** a return value
    - `.WaitBetweenAttempts(Func<int, Exception, TResult, TimeSpan>)` - Sets the delegate which will be used to calculate the wait time between the retry attempts.
    - `.WhenResultIs(Func<TResult, bool>)` - Sets the delegate which will be used to determine whether the given operation should be re-executed or not based on its return value.
    - `.OnRetry(Action<TResult, Exception, AttemptContext>)` - Sets the delegate which will be invoked when the given operation is being re-executed.
    - `.OnRetryAsync(Func<TResult, Exception, AttemptContext, CancellationToken, Task>)` - Sets the delegate which will be invoked asynchronously when the given operation is being re-executed.

> When the number of re-executions are reaching the configured maximum attempt count, the framework throws a custom `MaxRetryAttemptsReachedException` exception.

## Timeout

Ensures that the caller won't have to wait indefinitely for an operation to finish by setting a maximum time range within the given operation should be executed.

### Configuration

```c#
// Create a new bot policy
var policy = new BotPolicy();

// Or with a return value
var policy = new BotPolicy<HttpResponseMessage>();

// Configure the policy to retry failed operations
policy.Configure(policyConfig => policyConfig
    .Timeout(timeoutConfig => timeoutConfig
        // Set after how much time should be the given operation cancelled.
        .After(TimeSpan.FromSeconds(15))
        // (optional) Set a callback delegate which will be invoked when the 
        // given operation is timed out
        .OnTimeout((context) => Console.WriteLine("Operation timed out."))));
```

### Available configuration options
- `.After(TimeSpan)` - Sets after how much time should be the given operation cancelled.
- `.OnTimeout(Action<ExecutionContext>)` - Sets the delegate which will be invoked when the given operation is timing out.
- `.OnTimeoutAsync(Func<ExecutionContext, Task>)` - Sets the asynchronous delegate which will be invoked when the given operation is timing out.

> When the configured time is expired the framework throws a custom `OperationTimeoutException` exception.

## Fallback

Handles faults by executing an alternative operation when the original one is failing, also provides the ability to produce an alternative result value when the original operation is not able to do it.

### Configuration
    
- For operations **without** a return value
    ```c#
    // Create a new bot policy
    var policy = new BotPolicy();

    // Configure the policy to retry failed operations
    policy.Configure(policyConfig => policyConfig
        .Fallback(fallbackConfig => fallbackConfig
            // Set a delegate which will be used to determine whether the given fallback 
            // operation should be executed or not when a specific exception occurs.
            .WhenExceptionOccurs(exception => exception is HttpRequestException)
            // Set a delegate which will be invoked when the original operation is failed.
            .OnFallback((exception, context) => DoFallbackOperation())));
    ```
- For operations **with** a return value
    ```c#
    // Create a new bot policy
    var policy = new BotPolicy<HttpResponseMessage>();

    // Configure the policy to retry failed operations
    policy.Configure(policyConfig => policyConfig
        .Fallback(fallbackConfig => fallbackConfig
            // Set a delegate which will be used to determine whether the given fallback 
            // operation should be executed or not based on the originals return value.
            .WhenResultIs(result => result.StatusCode != HttpStatusCode.Ok)
            // Set a delegate which will be invoked asynchronously when the original operation is failed. 
            // It must provide an alternative return value.
            .OnFallback((result, exception, context) => DoFallbackOperation())));
    ```

### Available configuration options
- Same for policies **with or without** a return value
    - `.WhenExceptionOccurs(Func<Exception, bool>)` - Sets the delegate which will be used to determine whether the given fallback operation should be executed or not when a specific exception occurs.

- Only for policies **without** a return value
    - `.OnFallback(Action<Exception, ExecutionContext>)` - Sets the delegate which will be invoked when the original operation is failed.
    - `.OnFallbackAsync(Func<Exception, AttemptContext, CancellationToken, Task>)` - Sets the delegate which will be invoked asynchronously when the original operation is failed.
  
- Only for policies **with** a return value
    - `.WhenResultIs(Func<TResult, bool>)` - Sets the delegate which will be used to determine whether the given operation should be re-executed or not based on its return value.
    - `.OnFallback(Func<TResult, Exception, ExecutionContext, TResult>)` - Sets the delegate which will be invoked when the original operation is failed. Also provides an alternative return value.
    - `.OnFallbackAsync(Func<TResult, Exception, ExecutionContext, CancellationToken, Task<TResult>>)` - Sets the delegate which will be invoked asynchronously when the original operation is failed. Also provides an alternative return value.

## Circuit breaker

Prevents the continuous re-execution of a failing operation by blocking the traffic for a configured amount of time, when the number of failures exceed a given threshold. This usually could give some break to a remote resource to heal itself properly.

### Default implementation



## Execution of the configured policy
    
- With cancellation
    ```c#
    var tokenSource = new CancellationTokenSource();
    policy.Execute((context, cancellationToken) => DoSomeCancellableOperation(cancellationToken), tokenSource.Token);
    ```
- With a custom correlation id
    ```c#
    var correlationId = Guid.NewGuid();
    policy.Execute((context, cancellationToken) => DoSomeOperationWithCorrelationId(context.CorrelationId), correlationId);
    ```
    > Without setting a custom correlation id, the framework will always generate a unique one for every policy execution.
- Synchronously
    ```c#
    // Without lambda parameters
    policy.Execute(() => DoSomeOperation());

    // Or with lambda parameters
    policy.Execute((context, cancellationToken) => DoSomeOperation());
    ```
- Asynchronously
    ```c#
    // Without lambda parameters
    await policy.ExecuteAsync(() => DoSomeAsyncOperation());

    // Or with lambda parameters
    await policy.ExecuteAsync((context, cancellationToken) => DoSomeAsyncOperation());
    ```