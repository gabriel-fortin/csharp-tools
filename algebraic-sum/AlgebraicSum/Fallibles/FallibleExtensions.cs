namespace AlgebraicSum.Fallibles;

public static class FallibleExtensions
{
    /// <summary>
    /// Extracts the content of this <see cref="Fallible{TValue, TError}"/>
    /// when the types for success and error are the same
    /// </summary>
    public static T Unwrap<T>(this Fallible<T, T> fallible)
    {
        return fallible.Unwrap(x => x, x => x);
    }
}

/// <summmary>
/// Extensions for a <see cref="Fallible{TValue, TError}"/> wrapped in a <see cref="Task{TResult}"/>
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

    // <inheritdoc cref="Fallible.Then{TNextValue}(Func{TValue, TNextValue})"/>
    public static async Task<Fallible<TNextValue, TError>> Then<TValue, TError, TNextValue>(
        this Task<Fallible<TValue, TError>> fallibleTask,
        Func<TValue, TNextValue> mapper)
    {
        return (await fallibleTask).Then(mapper);
    }

    // <inheritdoc cref="Fallible{TValue, TError}.Then{TNextValue}(Func{TValue, Fallible{TNextValue, TError}})"/>
    public static async Task<Fallible<TNextValue, TError>> Then<TValue, TError, TNextValue>(
        this Task<Fallible<TValue, TError>> fallibleTask,
        Func<TValue, Fallible<TNextValue, TError>> wrappingMapper)
    {
        return (await fallibleTask).Then(wrappingMapper);
    }

    public static async Task<Fallible<TNextValue, TError>> Then<TValue, TError, TNextValue>(
        this Task<Fallible<TValue, TError>> fallibleTask,
        Func<TValue, Task<TNextValue>> asyncMapper)
    {
        Task<TNextValue> capturedResult = null!;
        await fallibleTask.Do(success => capturedResult = asyncMapper(success));
        return await capturedResult;
    }

    public static async Task<Fallible<TNextValue, TError>> Then<TValue, TError, TNextValue>(
        this Task<Fallible<TValue, TError>> fallibleTask,
        Func<TValue, Task<Fallible<TNextValue, TError>>> asyncWrappingMapper)
    {
        return await (await fallibleTask).Then(asyncWrappingMapper);
    }

    public static async Task<Fallible<TValue, TNextError>> OnError<TValue, TError, TNextError>(
        this Task<Fallible<TValue, TError>> fallibleTask,
        Func<TError, TNextError> mapper)
    {
        return (await fallibleTask).OnError(mapper);
    }

    public static async Task<Fallible<TValue, TNextError>> OnError<TValue, TError, TNextError>(
        this Task<Fallible<TValue, TError>> fallibleTask,
        Func<TError, Fallible<TValue, TNextError>> wrappingMapper)
    {
        return (await fallibleTask).OnError(wrappingMapper);
    }

    public static async Task<Fallible<TValue, TNextError>> OnError<TValue, TError, TNextError>(
        this Task<Fallible<TValue, TError>> fallibleTask,
        Func<TError, Task<TNextError>> asyncMapper)
    {
        Task<TNextError> capturedResult = default!;
        await fallibleTask.DoWithError(error => capturedResult = asyncMapper(error));
        return await capturedResult;
    }

    public static async Task<Fallible<TValue, TNextError>> OnError<TValue, TError, TNextError>(
        this Task<Fallible<TValue, TError>> fallibleTask,
        Func<TError, Task<Fallible<TValue, TNextError>>> asyncWrappingMapper)
    {
        return await (await fallibleTask).OnError(asyncWrappingMapper);
    }

    public static async Task<Fallible<TValue, TError>> Do<TValue, TError>(
        this Task<Fallible<TValue, TError>> fallibleTask,
        Action<TValue> action)
    {
        return (await fallibleTask).Do(action);
    }

    public static async Task<Fallible<TValue, TError>> Do<TValue, TError>(
        this Task<Fallible<TValue, TError>> fallibleTask,
        Func<TValue, Task> asyncAction)
    {
        return await (await fallibleTask).Do(asyncAction);
    }

    public static async Task<Fallible<TValue, TError>> DoWithError<TValue, TError>(
        this Task<Fallible<TValue, TError>> fallibleTask,
        Action<TError> action)
    {
        return (await fallibleTask).DoWithError(action);
    }

    public static async Task<Fallible<TValue, TError>> DoWithError<TValue, TError>(
        this Task<Fallible<TValue, TError>> fallibleTask,
        Func<TError, Task> asyncAction)
    {
        return await (await fallibleTask).DoWithError(asyncAction);
    }


    public static async Task<T> Unwrap<T>(this Task<Fallible<T, T>> fallibleTask)
    {
        return FallibleExtensions.Unwrap(await fallibleTask);
    }
}
