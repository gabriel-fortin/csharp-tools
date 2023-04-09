namespace AlgebraicSum.Fallibles;
// alternative names: Uncertain, Unreliable, YayOrNay

/// <summary>
/// Represents the result from an operation that can fail. It contains either a successful
/// or an erroneous value.
/// </summary>
public class Fallible<TSuccess, TError>
{
    private Sum<TSuccess, TError> internalValue;

    /// <summary>
    /// Constructs a <see cref="Fallible{T1, T2}"/> containing a successful value
    /// </summary>
    private Fallible(TSuccess success)
    {
        this.internalValue = success; // implicit cast
    }

    /// <summary>
    /// Constructs a <see cref="Fallible{T1, T2}"/> containing an erroneous value
    /// </summary>
    private Fallible(TError error)
    {
        this.internalValue = error; // implicit cast
    }

    /// <summary>
    /// If this object contains a successful value, runs the given <paramref name="mapper"/>
    /// on the value and returns a new <see cref="Fallible{T1, T2}"/> wrapping the
    /// <paramref name="mapper"/>'s return value as the new successful value.
    /// Otherwise, returns a new <see cref="Fallible{T1, T2}"/> wrapping the erroneous
    /// value from this object.
    /// </summary>
    public Fallible<TNextSuccess, TError> Then<TNextSuccess>(Func<TSuccess, TNextSuccess> mapper)
    {
        return internalValue.Reduce<Fallible<TNextSuccess, TError>>(
            success => mapper(success),
            error => error
        );
    }

    /// <summary>
    /// If this object contains a successful value, returns the result of running
    /// the given <paramref name="wrappingMapper"/> on the value.
    /// Otherwise, returns a new <see cref="Fallible{T1, T2}"/> wrapping the erroneous
    /// value from this object.
    /// </summary>
    public Fallible<TNextSuccess, TError> Then<TNextSuccess>(
        Func<TSuccess, Fallible<TNextSuccess, TError>> wrappingMapper)
    {
        return internalValue.Reduce<Fallible<TNextSuccess, TError>>(
            success => wrappingMapper(success),
            error => error
        );
    }

    /// <inheritdoc cref="Then{TNextSuccess}(Func{TSuccess, TNextSuccess})"/>
    /// <remarks>The resulting <see cref="Fallible{T1,T2}"/> is wrapped in a <see cref="Task"/></remarks>
    public async Task<Fallible<TNextSuccess, TError>> Then<TNextSuccess>(
        Func<TSuccess, Task<TNextSuccess>> asyncMapper)
    {
        return await Unwrap<Task<Fallible<TNextSuccess, TError>>>(
            whenSuccess: async success => await asyncMapper(success), // implicit cast
            whenError: error => Task.FromResult(Fallible<TNextSuccess, TError>.WrapError(error))
        );
    }

    /// <inheritdoc cref="Then{TNextSuccess}(Func{TSuccess, Fallible{TNextSuccess, TError}})"/>
    /// <remarks>The resulting <see cref="Fallible{T1,T2}"/> is wrapped in a <see cref="Task"/></remarks>
    public async Task<Fallible<TNextValue, TError>> Then<TNextValue>(
        Func<TSuccess, Task<Fallible<TNextValue, TError>>> asyncWrappingMapper)
    {
        return await Unwrap<Task<Fallible<TNextValue, TError>>>(
            whenSuccess: async success => await asyncWrappingMapper(success),
            whenError: error => Task.FromResult(Fallible<TNextValue, TError>.WrapError(error))
        );
    }

    /// <summary>
    /// If this object contains an erroneous value, runs the given <paramref name="mapper"/>
    /// on the value and returns a new <see cref="Fallible{T1, T2}"/> wrapping the
    /// <paramref name="mapper"/>'s return value as the new erroneous value.
    /// Otherwise, returns a new <see cref="Fallible{T1, T2}"/> wrapping the successful
    /// value from this object.
    /// </summary>
    public Fallible<TSuccess, TNextError> OnError<TNextError>(Func<TError, TNextError> mapper)
    {
        return internalValue.Reduce<Fallible<TSuccess, TNextError>>(
            success => success,
            error => mapper(error)
        );
    }

    /// <summary>
    /// If this object contains an erroneous value, returns the result of running
    /// the given <paramref name="wrappingMapper"/> on the value.
    /// Otherwise, returns a new <see cref="Fallible{T1, T2}"/> wrapping the successful
    /// value from this object.
    /// </summary>
    public Fallible<TSuccess, TNextError> OnError<TNextError>(
        Func<TError, Fallible<TSuccess, TNextError>> wrappingMapper)
    {
        return internalValue.Reduce<Fallible<TSuccess, TNextError>>(
            success => success,
            error => wrappingMapper(error)
        );
    }

    /// <inheritdoc cref="OnError{TNextError}(Func{TError, TNextError})"/>
    /// <remarks>The resulting <see cref="Fallible{T1,T2}"/> is wrapped in a <see cref="Task"/></remarks>
    public async Task<Fallible<TSuccess, TNextError>> OnError<TNextError>(
        Func<TError, Task<TNextError>> asyncMapper)
    {
        return await Unwrap<Task<Fallible<TSuccess, TNextError>>>(
            whenSuccess: success => Task.FromResult(Fallible<TSuccess, TNextError>.WrapSuccess(success)),
            whenError: async error => await asyncMapper(error) // implicit cast
        );
    }

    /// <inheritdoc cref="OnError{TNextError}(Func{TError, Fallible{TSuccess, TNextError}})"/>
    /// <remarks>The resulting <see cref="Fallible{T1,T2}"/> is wrapped in a <see cref="Task"/></remarks>
    public async Task<Fallible<TSuccess, TNextError>> OnError<TNextError>(
        Func<TError, Task<Fallible<TSuccess, TNextError>>> asyncWrappingMapper)
    {
        return await Unwrap<Task<Fallible<TSuccess, TNextError>>>(
            whenSuccess: success => Task.FromResult(Fallible<TSuccess, TNextError>.WrapSuccess(success)),
            whenError: async error => await asyncWrappingMapper(error)
        );
    }

    /// <summary>
    /// Side effect. <br/>
    /// If this object contains a successful value, runs the given <paramref name="action"/> on the value.
    /// Returns this object.
    /// </summary>
    public Fallible<TSuccess, TError> Do(Action<TSuccess> action)
    {
        internalValue.Use(action, x => {});
        return this;
    }

    /// <inheritdoc cref="Do(Action{TSuccess})"/>
    /// <remarks>The resulting <see cref="Fallible{T1,T2}"/> is wrapped in a <see cref="Task"/></remarks>
    public async Task<Fallible<TSuccess, TError>> Do(Func<TSuccess, Task> asyncAction)
    {
        TSuccess capturedValue = default!;
        internalValue.Use(success => capturedValue = success, x => {});
        await asyncAction(capturedValue);
        return this;
    }

    /// <summary>
    /// Side effect. <br/>
    /// If this object contains an erroneous value, runs the given <paramref name="action"/> on the value.
    /// Returns this object.
    /// </summary>
    public Fallible<TSuccess, TError> DoWithError(Action<TError> action)
    {
        internalValue.Use(x => {}, action);
        return this;
    }

    /// <inheritdoc cref="DoWithError(Action{TError})(Action{TSuccess})"/>
    /// <remarks>The resulting <see cref="Fallible{T1,T2}"/> is wrapped in a <see cref="Task"/></remarks>
    public async Task<Fallible<TSuccess, TError>> DoWithError(Func<TError, Task> asyncAction)
    {
        TError capturedValue = default!;
        internalValue.Use(x => {}, error => capturedValue = error);
        await asyncAction(capturedValue);
        return this;
    }

    /// <summary>
    /// Extracts the content of this <see cref="Fallible{T1, T2}"/>
    /// using one of the given unwrapping mappers. The choice of mapper
    /// is made based on the value in this object being a success or an error.
    /// </summary>
    public T Unwrap<T>(Func<TSuccess, T> whenSuccess, Func<TError, T> whenError)
    {
        return internalValue.Reduce(whenSuccess, whenError);
    }

    /// <summary>
    /// Extracts the content of this <see cref="Fallible{T1, T2}"/>.
    /// If this object contains an erroneous value, returns the result of running
    /// the given <paramref name="whenError"/> mapper on the value.
    /// Otherwise, returns the successful value as-is.
    /// </summary>
    public TSuccess Unwrap(Func<TError, TSuccess> whenError)
    {
        return Unwrap<TSuccess>(x => x, whenError);
    }

    /// <summary>
    /// Extracts the content of this <see cref="Fallible{T1, T2}"/>.
    /// If this object contains a successful value, returns the result of running
    /// the given <paramref name="whenSuccess"/> mapper on the value.
    /// Otherwise, returns the erroneous value as-is.
    /// </summary>
    public TError Unwrap(Func<TSuccess, TError> whenSuccess)
    {
        return Unwrap<TError>(whenSuccess, x => x);
    }

    // potential additional methods
    //      TSuccess UnwrapSuccess()  // can throw
    //      TError UnwrapError()  // can throw
    //      bool TryUnwrapSuccess(out TSuccess s)
    //      bool TryUnwrapError(out TError e)

    /// <summary>
    /// Creates a <see cref="Fallible{TSuccess, TError}"/>
    /// that wraps the given <paramref name="success"/> value.
    /// </summary>
    public static Fallible<TSuccess, TError> WrapSuccess(TSuccess success)
    {
        return new Fallible<TSuccess, TError>(success);
    }

    /// <summary>
    /// Creates a <see cref="Fallible{TSuccess, TError}"/>
    /// that wraps the given <paramref name="error"/> value.
    /// </summary>
    public static Fallible<TSuccess, TError> WrapError(TError error)
    {
        return new Fallible<TSuccess, TError>(error);
    }


    /// enables implicit casting of a <typeparamref name="TSuccess"/> object
    /// into a <see cref="Fallible{T1, T2}"/>
    public static implicit operator Fallible<TSuccess, TError>(TSuccess success) =>
        WrapSuccess(success);

    /// enables implicit casting of a <typeparamref name="TError"/> object
    /// into a <see cref="Fallible{T1, T2}"/>
    public static implicit operator Fallible<TSuccess, TError> (TError error) =>
        WrapError(error);
}
