namespace AlgebraicSum.Fallibles;

public static class FallibleExtensions
{
    /// <summary>
    /// Extracts the content of this <see cref="Fallible{T1, T2}"/>
    /// when the type of the success value is the same as the type of the error value
    /// </summary>
    public static T Unwrap<T>(this Fallible<T, T> fallible)
    {
        return fallible.Unwrap(x => x, x => x);
    }
}

/// <summmary>
/// Extensions for a <see cref="Fallible{T1, T2}"/> wrapped in a <see cref="Task{TResult}"/>
/// </summmary>
public static class FallibleAsyncExtensions
{
    // There is very little variance in the parameters but the number of parameters and methods
    // results in a lot of combinations (it's exponential in the number of parameters/methods)
    // What can vary though:
    // - receiver: always async: a Task<Fallible<X, Y>>
    // - mapper's result: can be wrapped or plain: a Fallible<X, Y> or X
    // - mapper's result: can be async or sync: Task<Z> or Z
    // - method: can be .Then, .OnError, .Do, .DoOnError

    /// <inheritdoc cref="Fallible{TSuccess, TError}.Then{TNextSuccess}(Func{TSuccess, TNextSuccess})"/>
    public static async Task<Fallible<TNextSuccess, TError>> Then<TSuccess, TError, TNextSuccess>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TSuccess, TNextSuccess> mapper)
    {
        return (await fallibleTask).Then(mapper);
    }

    /// <inheritdoc cref="Fallible{TSuccess, TError}.Then{TNextSuccess}(Func{TNextSuccess, Fallible{TNextSuccess, TError}})"/>
    public static async Task<Fallible<TNextSuccess, TError>> Then<TSuccess, TError, TNextSuccess>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TSuccess, Fallible<TNextSuccess, TError>> wrappingMapper)
    {
        return (await fallibleTask).Then(wrappingMapper);
    }

    public static async Task<Fallible<TNextSuccess, TError>> Then<TSuccess, TError, TNextSuccess>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TSuccess, Task<TNextSuccess>> asyncMapper)
    {
        Task<TNextSuccess> capturedResult = null!;
        await fallibleTask.Do(success => capturedResult = asyncMapper(success));
        return await capturedResult;
    }

    public static async Task<Fallible<TNextSuccess, TError>> Then<TSuccess, TError, TNextSuccess>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TSuccess, Task<Fallible<TNextSuccess, TError>>> asyncWrappingMapper)
    {
        return await (await fallibleTask).Then(asyncWrappingMapper);
    }

    public static async Task<Fallible<TSuccess, TNextError>> OnError<TSuccess, TError, TNextError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TError, TNextError> mapper)
    {
        return (await fallibleTask).OnError(mapper);
    }

    public static async Task<Fallible<TSuccess, TNextError>> OnError<TSuccess, TError, TNextError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TError, Fallible<TSuccess, TNextError>> wrappingMapper)
    {
        return (await fallibleTask).OnError(wrappingMapper);
    }

    public static async Task<Fallible<TSuccess, TNextError>> OnError<TSuccess, TError, TNextError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TError, Task<TNextError>> asyncMapper)
    {
        Task<TNextError> capturedResult = default!;
        await fallibleTask.DoWithError(error => capturedResult = asyncMapper(error));
        return await capturedResult;
    }

    public static async Task<Fallible<TSuccess, TNextError>> OnError<TSuccess, TError, TNextError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TError, Task<Fallible<TSuccess, TNextError>>> asyncWrappingMapper)
    {
        return await (await fallibleTask).OnError(asyncWrappingMapper);
    }

    public static async Task<Fallible<TSuccess, TError>> Do<TSuccess, TError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Action<TSuccess> action)
    {
        return (await fallibleTask).Do(action);
    }

    public static async Task<Fallible<TSuccess, TError>> Do<TSuccess, TError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TSuccess, Task> asyncAction)
    {
        return await (await fallibleTask).Do(asyncAction);
    }

    public static async Task<Fallible<TSuccess, TError>> DoWithError<TSuccess, TError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Action<TError> action)
    {
        return (await fallibleTask).DoWithError(action);
    }

    public static async Task<Fallible<TSuccess, TError>> DoWithError<TSuccess, TError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TError, Task> asyncAction)
    {
        return await (await fallibleTask).DoWithError(asyncAction);
    }


    public static async Task<T> Unwrap<T>(this Task<Fallible<T, T>> fallibleTask)
    {
        return FallibleExtensions.Unwrap(await fallibleTask);
    }
}
