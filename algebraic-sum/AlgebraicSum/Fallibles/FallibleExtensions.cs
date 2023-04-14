namespace AlgebraicSum.Fallibles;

/// <summary>
/// Extensions for a <see cref="Fallible{T1, T2}"/> wrapped in a <see cref="Task{TResult}"/>
/// </summary>
public static class FallibleAsyncExtensions
{
    // There is little variance in the parameters but the number of parameters and methods
    // results in a lot of combinations (it's exponential in the number of parameters/methods)
    // What cannot vary:
    // - receiver: always async; Task<Fallible<X, Y>>
    // What can vary:
    // - mapper's result: can be wrapped or plain; a Fallible<X, Y> or X
    // - mapper's result: can be async or sync; Task<Z> or Z
    // - method: can be `.Then`, `.OnError`, `.Do`, or `.DoOnError`


    /***** THEN methods ********************************************/

    /// <inheritdoc cref="Fallible{TSuccess, TError}.Then{TNextSuccess}(Func{TSuccess, Task{TNextSuccess}})"/>
    public static async Task<Fallible<TNextSuccess, TError>> Then<TSuccess, TError, TNextSuccess>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TSuccess, TNextSuccess> mapper)
    {
        return (await fallibleTask).Then(mapper);
    }

    /// <inheritdoc cref="Fallible{TSuccess, TError}.Then{TNextSuccess}(Func{TSuccess, Task{Fallible{TNextSuccess, TError}}})"/>
    public static async Task<Fallible<TNextSuccess, TError>> Then<TSuccess, TError, TNextSuccess>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TSuccess, Fallible<TNextSuccess, TError>> wrappingMapper)
    {
        return (await fallibleTask).Then(wrappingMapper);
    }

    /// <inheritdoc cref="Fallible{TSuccess, TError}.Then{TNextSuccess}(Func{TSuccess, Task{TNextSuccess}})"/>
    public static async Task<Fallible<TNextSuccess, TError>> Then<TSuccess, TError, TNextSuccess>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TSuccess, Task<TNextSuccess>> asyncMapper)
    {
        Task<TNextSuccess> capturedResult = null!;
        await fallibleTask.Do(success => capturedResult = asyncMapper(success));
        return await capturedResult;
    }

    /// <inheritdoc cref="Fallible{TSuccess, TError}.Then{TNextSuccess}(Func{TSuccess, Task{Fallible{TNextSuccess, TError}}})"/>
    public static async Task<Fallible<TNextSuccess, TError>> Then<TSuccess, TError, TNextSuccess>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TSuccess, Task<Fallible<TNextSuccess, TError>>> asyncWrappingMapper)
    {
        return await (await fallibleTask).Then(asyncWrappingMapper);
    }


    /***** ON ERROR methods ********************************************/

    /// <inheritdoc cref="Fallible{TSuccess, TError}.OnError{TNextError}(Func{TError, Task{TNextError}})"/>
    public static async Task<Fallible<TSuccess, TNextError>> OnError<TSuccess, TError, TNextError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TError, TNextError> mapper)
    {
        return (await fallibleTask).OnError(mapper);
    }

    /// <inheritdoc cref="Fallible{TSuccess, TError}.OnError{TNextError}(Func{TError, Task{Fallible{TSuccess, TNextError}}})"/>
    public static async Task<Fallible<TSuccess, TNextError>> OnError<TSuccess, TError, TNextError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TError, Fallible<TSuccess, TNextError>> wrappingMapper)
    {
        return (await fallibleTask).OnError(wrappingMapper);
    }

    /// <inheritdoc cref="Fallible{TSuccess, TError}.OnError{TNextError}(Func{TError, Task{TNextError}})"/>
    public static async Task<Fallible<TSuccess, TNextError>> OnError<TSuccess, TError, TNextError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TError, Task<TNextError>> asyncMapper)
    {
        Task<TNextError> capturedResult = default!;
        await fallibleTask.DoWithError(error => capturedResult = asyncMapper(error));
        return await capturedResult;
    }

    /// <inheritdoc cref="Fallible{TSuccess, TError}.OnError{TNextError}(Func{TError, Task{Fallible{TSuccess, TNextError}}})"/>
    public static async Task<Fallible<TSuccess, TNextError>> OnError<TSuccess, TError, TNextError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TError, Task<Fallible<TSuccess, TNextError>>> asyncWrappingMapper)
    {
        return await (await fallibleTask).OnError(asyncWrappingMapper);
    }


    /***** DO methods ********************************************/

    /// <inheritdoc cref="Fallible{TSuccess, TError}.Do(Func{TSuccess, Task})"/>
    public static async Task<Fallible<TSuccess, TError>> Do<TSuccess, TError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Action<TSuccess> action)
    {
        return (await fallibleTask).Do(action);
    }

    /// <inheritdoc cref="Fallible{TSuccess, TError}.Do(Func{TSuccess, Task})"/>
    public static async Task<Fallible<TSuccess, TError>> Do<TSuccess, TError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TSuccess, Task> asyncAction)
    {
        return await (await fallibleTask).Do(asyncAction);
    }


    /***** DO WITH ERROR methods ********************************************/

    /// <inheritdoc cref="Fallible{TSuccess, TError}.DoWithError(Func{TError, Task})"/>
    public static async Task<Fallible<TSuccess, TError>> DoWithError<TSuccess, TError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Action<TError> action)
    {
        return (await fallibleTask).DoWithError(action);
    }

    /// <inheritdoc cref="Fallible{TSuccess, TError}.DoWithError(Func{TError, Task})"/>
    public static async Task<Fallible<TSuccess, TError>> DoWithError<TSuccess, TError>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TError, Task> asyncAction)
    {
        return await (await fallibleTask).DoWithError(asyncAction);
    }


    /***** UNWRAP methods ********************************************/

    /// <inheritdoc cref="Fallible{TSuccess, TError}.Unwrap{T}(Func{TSuccess, T}, Func{TError, T})"/>
    /// <remarks>The result is wrapped in a <see cref="Task"/></remarks>
    public static async Task<TResult> Unwrap<TSuccess, TError, TResult>(
        this Task<Fallible<TSuccess, TError>> fallibleTask,
        Func<TSuccess, TResult> whenSuccess,
        Func<TError, TResult> whenError)
    {
        return (await fallibleTask).Unwrap(whenSuccess, whenError);
    }

    /// <summary>
    /// Extracts the content of this <see cref="Fallible{T1, T2}"/>
    /// when the type of the successful value is the same as the type of the erroneous value
    /// </summary>
    public static T Unwrap<T>(this Fallible<T, T> fallible)
    {
        return fallible.Unwrap(x => x, x => x);
    }

    /// <inheritdoc cref="FallibleExtensions.Unwrap{T}(Fallible{T, T})"/>
    /// <remarks>The result is wrapped in a <see cref="Task"/></remarks>
    public static async Task<T> Unwrap<T>(this Task<Fallible<T, T>> fallibleTask)
    {
        return Unwrap(await fallibleTask);
    }
}
