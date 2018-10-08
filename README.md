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

// Configure the policy to use timeout on operations
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

    // Configure the policy to use fallback
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

    // Configure the policy to use fallback
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

Prevents the continuous re-execution of a failing operation by blocking the traffic for a configured amount of time, when the number of failures exceed a given threshold. This usually could give some break to the remote resource to heal itself properly.

### Default implementation

Implements the Circuit Breaker pattern described [here](https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker). It can be used by configuring the `Botpolicy` like this:

- For operations **without** a return value
    ```c#
    // Create a new bot policy
    var policy = new BotPolicy();

    // Configure the policy to use the default circuit breaker
    policy.Configure(policyConfig => policyConfig
        .CircuitBreaker(circuitBreakerConfig => circuitBreakerConfig

            // Sets the amount of time of how long the circuit breaker should 
            // remain in the Open state before turning into HalfOpen.
            .DurationOfOpen(TimeSpan.FromSeconds(15))

            // Sets the delegate which will be used to determine whether the 
            // given operation should be marked as failed when a specific exception occurs.
            .BrakeWhenExceptionOccurs(exception => exception is HttpRequestException),

                // Configure the default circuit breaker strategy
                strategyConfig => strategyConfig

                    // Sets the maximum number of failed operations before 
                    // the circuit breaker turns into the Open state.
                    .FailureThresholdBeforeOpen(5)

                    // Sets the minimum number of succeeded operations should 
                    // be reached when the circuit breaker is in HalfOpen state 
                    // before turning into Closed.
                    .SuccessThresholdInHalfOpen(2)));
    ```
- For operations **with** a return value
    ```c#
    // Create a new bot policy
    var policy = new BotPolicy<HttpResponseMessage>();

    // Configure the policy to use the default circuit breaker
    policy.Configure(policyConfig => policyConfig
        .CircuitBreaker(circuitBreakerConfig => circuitBreakerConfig

            // Sets the amount of time of how long the circuit breaker should 
            // remain in the Open state before turning into HalfOpen.
            .DurationOfOpen(TimeSpan.FromSeconds(15))

            // Sets the delegate which will be used to determine whether the 
            // given operation should be marked as failed based on its return value.
            .BrakeWhenResultIs(result => !result.IsSucceeded),

                // Configure the default circuit breaker strategy
                strategyConfig => strategyConfig

                    // Sets the maximum number of failed operations before 
                    // the circuit breaker turns into the Open state.
                    .FailureThresholdBeforeOpen(5)

                    // Sets the minimum number of succeeded operations should 
                    // be reached when the circuit breaker is in HalfOpen state 
                    // before turning into Closed.
                    .SuccessThresholdInHalfOpen(2)));
    ```

### Custom implementation

The functionality of the Circuit Breaker bot can be extended with a custom `CircuitBreakerStrategy` implementation. It can give more control over the internal behavior by determining when should the Circuit switch between its states.
 ```c#
class CustomCircuitBreakerStrategy : CircuitBreakerStrategy
{
    public DefaultCircuitBreakerStrategy(CircuitBreakerConfigurationBase config)
        :base(config)
    {}

    // Called when the underlying operation is failed within the Closed circuit state.
    // Returns true if the circuit should open, otherwise false.
    protected override bool OperationFailedInClosed()
    {
        // You can count here how many executions failed in the Closed state.
    }

    // Called when the underlying operation is failed within the HalfOpen circuit state.
    // Should return true if the circuit should move back to open 
    // state from half open, otherwise false.
    protected override bool OperationFailedInHalfOpen()
    {
        // You can decide what should happen when an operation fails in
        // the HalfOpen state, remain in HalfOpen or open the circuit again.
    }

    // Called when the underlying operation is succeeded within the HalfOpen circuit state.
    // Returns true if the circuit should be closed, otherwise false.
    protected override bool OperationSucceededInHalfOpen()
    {
        // You can decide what should happen when an operation succeeds in
        // the HalfOpen state, close the circuit or remain in half open for a while.
    }

    protected override void Reset()
    {
        // Called when the underlying operation is succeeded within the Closed circuit state.
    }
}
```
Then you can use your custom strategy by configuring the `Botpolicy` like this:
```c#
policy.Configure(policyConfig => policyConfig
    .CustomCircuitBreaker(circuitBreakerConfig => 

        // Construct your custom implementation
        new CustomCircuitBreakerStrategy(circuitBreakerConfig),

            // Configure the circuit breaker bot
            circuitBreakerConfig => circuitBreakerConfig

            // Sets the amount of time of how long the circuit breaker should 
            // remain in the Open state before turning into HalfOpen.
            .DurationOfOpen(TimeSpan.FromSeconds(15))

            // Sets the delegate which will be used to determine whether the 
            // given operation should be marked as failed based on its return value.
            .BrakeWhenResultIs(result => !result.IsSucceeded),

                // Configure the default circuit breaker strategy
                strategyConfig => strategyConfig

                    // Sets the maximum number of failed operations before 
                    // the circuit breaker turns into the Open state.
                    .FailureThresholdBeforeOpen(5)

                    // Sets the minimum number of succeeded operations should 
                    // be reached when the circuit breaker is in HalfOpen state 
                    // before turning into Closed.
                    .SuccessThresholdInHalfOpen(2)));
```

### Available configuration options
- Same for policies **with or without** a return value
    - `.BrakeWhenExceptionOccurs(Func<Exception, bool>)` - Sets the delegate which will be used to determine whether the given operation should be marked as failed by the `CircuitBreakerStrategy` or not when a specific exception occurs.
    - `.DurationOfOpen(TimeSpan openStateDuration)` - Sets the amount of time of how long the circuit breaker should remain in the Open state before turning into HalfOpen.
    - `.WithStateHandler(ICircuitStateHandler stateHandler)` - Sets a the underlying [circuit state handler](#circuit-state-handler) implementation.
    - `.OnOpen(Action<TimeSpan> openHandler)` - Sets the delegate which will be invoked when the circuit breaker trips to the open state.
    - `.OnClosed(Action closedHandler)` - Sets the delegate which will be invoked when the circuit breaker trips to the closed state.
    - `.OnHalfOpen(Action halfOpenHandler)` - Sets the delegate which will be invoked when the circuit breaker trips to the half open state.

- Only for policies **with** a return value
    - `.BrakeWhenResultIs(Func<TResult, bool>)` - Sets the delegate which will be used to determine whether the given operation should be marked as failed by the `CircuitBreakerStrategy` or not based on its return value.

> When an operation execution attempt is made when the Circuit Breaker is in Open state, it throws a `CircuitOpenException` exception.

> When there is more than one operation executing during the HalfOpen state, the second operation would be interrupted with a `HalfOpenExecutionLimitExceededException` exception.

### Circuit state handler

The circuit state handler is an other abstraction which allows the customization of the Circuit Breakers internal behavior. Usually it could be used to store the actual Circuit State, however there are other use cases where it could be very useful when you have the ability to modify the handling of that state. As an example you can check the [DistributedCircuitStateHandler](https://github.com/z4kn4fein/trybot/tree/master/sandbox/trybot.distributedcb) implementation used to simulate the case when the Circuit State is shared between several Circuit Breakers in a distributed cache.

If you'd like to create a custom state handler you have to implement the `ICircuitStateHandler` interface:
```c#
public class DistributedCircuitStateHandler : ICircuitStateHandler
    {
        public CircuitState Read()
        {
            // Reads the current circuit state.
        }

        public void Update(CircuitState state)
        {
            // Updates the actual state, it's called when the CircuitBreakerStrategy
            // chooses to switch to another state.
        }

        public Task<CircuitState> ReadAsync(CancellationToken token, bool continueOnCapturedContext)
        {
            // Reads the current circuit state asynchronously.            
        }

        public Task UpdateAsync(CircuitState state, CancellationToken token, bool continueOnCapturedContext)
        {
             // Updates the actual state asynchronously.
        }
    }
```

Then you can set your custom implementation like this:

```c#
policy.Configure(policyConfig => policyConfig
    .CircuitBreaker(circuitBreakerConfig => circuitBreakerConfig
        .WithStateHandler(new DistributedCircuitStateHandler()), /* strategy config */));
```

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

## Custom Bots

If you are facing a use case which is not covered by the built-in bots in Trybot, you have the option to make your own bot. All you have to do is to inherit from one of the `Bot`, `Bot<TResult>`, `ConfigurableBot`, `ConfigurableBot<TResult>` abstract classes.

They are for differenct use cases:
- **`Bot`**: Inheriting from this allows you to create a bot **without configuration** which can handle operations **without return value**.
    ```c#
    class CustomBot : Bot
    {
        internal CustomBot(Bot innerBot) // the inner bot is the next bot in the chain
            : base(innerBot)
        { }

        public override void Execute(IBotOperation operation, ExecutionContext context, CancellationToken token)
        { 
            // here you can use your custom handling logic on the inner bots call:
            // base.InnerBot.Execute(operation, context, token);
        }

        public override Task ExecuteAsync(IAsyncBotOperation operation, ExecutionContext context, CancellationToken token)
        { 
            // here you can use your custom handling logic on the inner bots call:
            // base.InnerBot.ExecuteAsync(operation, context, token);
        }
    }
    ```
- **`Bot<TResult>`**: Inheriting from this allows you to create a bot **without configuration** which can handle operations **with return value**.
    ```c#
    class CustomBot<TResult> : Bot<TResult>
    {
        internal CustomBot(Bot<TResult> innerBot) // the inner bot is the next bot in the chain
            : base(innerBot)
        { }

        public override TResult Execute(IBotOperation<TResult> operation, ExecutionContext context, CancellationToken token)
        { 
            // here you can use your custom handling logic on the inner bots call:
            // return base.InnerBot.Execute(operation, context, token);
        }

        public override Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, ExecutionContext context, CancellationToken token)
        { 
            // here you can use your custom handling logic on the inner bots call:
            // return base.InnerBot.ExecuteAsync(operation, context, token);
        }
    }
    ```
- **`ConfigurableBot`**: Inheriting from this allows you to create a bot **with configuration** which can handle operations **without return value**.
    ```c#
    class CustomBot : ConfigurableBot<CustomConfiguration>
    {
        internal CustomBot(Bot innerBot, CustomConfiguration configuration) 
            : base(innerBot, configuration) // the inner bot is the next bot in the chain
        { }

        public override void Execute(IBotOperation operation, ExecutionContext context, CancellationToken token)
        { 
            // here you can use your custom handling logic on the inner bots call:
            // base.InnerBot.Execute(operation, context, token);
        }

        public override Task ExecuteAsync(IAsyncBotOperation operation, ExecutionContext context, CancellationToken token)
        { 
            // here you can use your custom handling logic on the inner bots call:
            // base.InnerBot.ExecuteAsync(operation, context, token);
        }
    }
    ```
- **`ConfigurableBot<TResult>`**: Inheriting from this allows you to create a bot **with configuration** which can handle operations **with return value**.
    ```c#
    class CustomBot<TResult, CustomConfiguration> : ConfigurableBot<CustomConfiguration, TResult>
    {
        internal CustomBot(Bot<TResult> innerBot, CustomConfiguration configuration) 
            : base(innerBot, configuration) // the inner bot is the next bot in the chain
        { }

        public override TResult Execute(IBotOperation<TResult> operation, ExecutionContext context, CancellationToken token)
        { 
            // here you can use your custom handling logic on the inner bots call:
            // return base.InnerBot.Execute(operation, context, token);
        }

        public override Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, ExecutionContext context, CancellationToken token)
        { 
            // here you can use your custom handling logic on the inner bots call:
            // return base.InnerBot.ExecuteAsync(operation, context, token);
        }
    }
    ```

Then you can register your custom bot into a `BotPolicy` like this:
- In case of a simple bot
    ```c#
    policy.Configure(policyConfig => policyConfig
        .AddBot(innerBot => new CustomBot(innerBot));
    ```
- In case of a bot with configuration
    ```c#
    policy.Configure(policyConfig => policyConfig
        .AddBot((innerBot, config) => new CustomBot(innerBot, config), new CustomConfiguration());
    ```

## Bot chains

During the configuration of a bot policy you can chain different bots to eachother.

```c#
policy.Configure(policyConfig => policyConfig
    .CircuitBreaker(circuitBreakerConfig => circuitBreakerConfig
        .DurationOfOpen(TimeSpan.FromSeconds(10))
        .BrakeWhenExceptionOccurs(exception => exception is HttpRequestException),
            strategyConfig => strategyConfig
                .FailureThresholdBeforeOpen(5)
                .SuccessThresholdInHalfOpen(2))
    .Retry(retryConfig => retryConfig
        .WithMaxAttemptCount(5)
        .WhenExceptionOccurs(exception => exception is HttpRequestException)
        .WaitBetweenAttempts((attempt, exception) => 
        {
            if(exception is CircuitOpenException cbException)
                return TimeSpan.FromSeconds(cbException.OpenDuration);

            return TimeSpan.FromSeconds(Math.Pow(2, attempt);
        })))
    .Timeout(timeoutConfig => timeoutConfig
        .After(TimeSpan.FromSeconds(120))));
```

The handling order of the given operation would be the same as the configuration order. So from the top to the bottom, which means in the example above the circuit breaker will try to execute the given operation first, then if it fails the retry bot will re-execute it until the timeout is not firing a cancellation.