namespace AlgebraicSum.Fallibles;
// alternative names: Uncertain, Unreliable, YayOrNay

/// <summary>
/// Represents the result from an operation that can fail. It contains either a successful
/// or an erroneous value.
/// </summary>
public class Fallible<TValue, TError>
{
    private Sum<TValue, TError> internalValue;

    /// <summary>
    /// Constructs a <see cref="Fallible{T1, T2}"/> containing a successful value
    /// </summary>
    private Fallible(TValue value)
    {
        this.internalValue = value; // implicit cast
    }

    /// <summary>
    /// Constructs a <see cref="Fallible{T1, T2}"/> containing an erroneous value
    /// </summary>
    private Fallible(TError error)
    {
        this.internalValue = error; // implicit cast
    }

    /// <summary>
    /// If this object contains a successful value, runs the given <paramref name="mapper"/> and
    /// returns a new <see cref="Fallible{T1, T2}"/> wrapping the <paramref name="mapper"/>'s return
    /// value as the new successful value.
    /// Otherwise, returns a new <see cref="Fallible{T1, T2}"/> wrapping the erroneous
    /// value from this object.
    /// </summary>
    public Fallible<TNextValue, TError> Then<TNextValue>(Func<TValue, TNextValue> mapper)
    {
        return internalValue.Reduce<Fallible<TNextValue, TError>>(
            value => mapper(value),
            error => error
        );
    }

    /// <summary>
    /// If this object contains a successful value, returns the result of running
    /// the given <paramref name="wrappingMapper"/>.
    /// Otherwise, returns a new <see cref="Fallible{T1, T2}"/> wrapping the erroneous
    /// value from this object.
    /// </summary>
    public Fallible<TNextValue, TError> Then<TNextValue>(Func<TValue, Fallible<TNextValue, TError>> wrappingMapper)
    {
        return internalValue.Reduce<Fallible<TNextValue, TError>>(
            value => wrappingMapper(value),
            error => error
        );
    }

    /// <summary>
    /// AAAAAAA TODO
    /// </summary>
    /// <remarks>The resulting <see cref="Fallible{T1,T2}"/> is wrapped in a <see cref="Task"/></remarks>
    public async Task<Fallible<TNextValue, TError>> Then<TNextValue>(Func<TValue, Task<TNextValue>> asyncMapper)
    {
        return await Unwrap<Task<Fallible<TNextValue, TError>>>(
            whenValue: async value => await asyncMapper(value), // implicit cast
            whenError: error => Task.FromResult(Fallible<TNextValue, TError>.WrapError(error))
        );
    }

    /// <summary>
    /// BBBBBBB TODO
    /// </summary>
    /// <remarks>The resulting <see cref="Fallible{T1,T2}"/> is wrapped in a <see cref="Task"/></remarks>
    public async Task<Fallible<TNextValue, TError>> Then<TNextValue>(
        Func<TValue, Task<Fallible<TNextValue, TError>>> asyncWrappingMapper)
    {
        return await Unwrap<Task<Fallible<TNextValue, TError>>>(
            whenValue: async value => await asyncWrappingMapper(value),
            whenError: error => Task.FromResult(Fallible<TNextValue, TError>.WrapError(error))
        );
    }

    /// <summary>
    /// If this object contains an erroneous value, runs the given <paramref name="mapper"/> and
    /// returns a new <see cref="Fallible{T1, T2}"/> wrapping the <paramref name="mapper"/>'s return
    /// value as the new erroneous value.
    /// Otherwise, returns a new <see cref="Fallible{T1, T2}"/> wrapping the successful
    /// value of this object.
    /// </summary>
    public Fallible<TValue, TNextError> OnError<TNextError>(Func<TError, TNextError> mapper)
    {
        return internalValue.Reduce<Fallible<TValue, TNextError>>(
            value => value,
            error => mapper(error)
        );
    }

    /// <summary>
    /// If this object contains an erroneous value, returns the result of running
    /// the given <paramref name="wrappingMapper"/>.
    /// Otherwise, returns a new <see cref="Fallible{T1, T2}"/> wrapping the successful
    /// value as this object.
    /// </summary>
    public Fallible<TValue, TNextError> OnError<TNextError>(Func<TError, Fallible<TValue, TNextError>> wrappingMapper)
    {
        return internalValue.Reduce<Fallible<TValue, TNextError>>(
            value => value,
            error => wrappingMapper(error)
        );
    }

    /// <remarks>The resulting <see cref="Fallible{T1,T2}"/> is wrapped in a <see cref="Task"/></remarks>
    public async Task<Fallible<TValue, TNextError>> OnError<TNextError>(Func<TError, Task<TNextError>> asyncMapper)
    {
        return await Unwrap<Task<Fallible<TValue, TNextError>>>(
            whenValue: value => Task.FromResult(Fallible<TValue, TNextError>.WrapValue(value)),
            whenError: async error => await asyncMapper(error) // implicit cast
        );
    }

    /// <remarks>The resulting <see cref="Fallible{T1,T2}"/> is wrapped in a <see cref="Task"/></remarks>
    public async Task<Fallible<TValue, TNextError>> OnError<TNextError>(
        Func<TError, Task<Fallible<TValue, TNextError>>> asyncWrappingMapper)
    {
        return await Unwrap<Task<Fallible<TValue, TNextError>>>(
            whenValue: value => Task.FromResult(Fallible<TValue, TNextError>.WrapValue(value)),
            whenError: async error => await asyncWrappingMapper(error)
        );
    }

    /// <summary>
    /// Side effect. <br/>
    /// If this object contains a successful value, runs the given <paramref name="action"/>.
    /// Returns this object.
    /// </summary>
    public Fallible<TValue, TError> Do(Action<TValue> action)
    {
        internalValue.Use(action, x => {});
        return this;
    }

    /// <summary>
    /// CCCCC  // when the action is awaitable (for example audit that makes an HTTP call)
    /// </summary>
    /// <remarks>The resulting <see cref="Fallible{T1,T2}"/> is wrapped in a <see cref="Task"/></remarks>
    public async Task<Fallible<TValue, TError>> Do(Func<TValue, Task> asyncAction)
    {
        TValue capturedValue = default!;
        internalValue.Use(success => capturedValue = success, x => {});
        await asyncAction(capturedValue);
        return this;
    }

    /// <summary>
    /// Side effect. <br/>
    /// If this object contains an erroneous value, runs the given <paramref name="action"/>.
    /// Returns this object.
    /// </summary>
    public Fallible<TValue, TError> DoWithError(Action<TError> action)
    {
        internalValue.Use(x => {}, action);
        return this;
    }

    /// <summary>
    /// DDDDDD  // when the action is awaitable (for example audit that makes an HTTP call)
    /// </summary>
    public async Task<Fallible<TValue, TError>> DoWithError(Func<TError, Task> asyncAction)
    {
        TError captiredError = default!;
        internalValue.Use(x => {}, error => captiredError = error);
        await asyncAction(captiredError);
        return this;
    }

    /// <summary>
    /// Extracts the content of this <see cref="Fallible{TValue, TError}"/>
    /// using the given unwrapping mappers
    /// </summary>
    public T Unwrap<T>(Func<TValue, T> whenValue, Func<TError, T> whenError)
    {
        return internalValue.Reduce(whenValue, whenError);
    }

    // potential additional methods
    //      TValue UnwrapValue()  // can throw
    //      TError UnwrapError()  // can throw
    //      bool TryUnwrapValue(out TValue v)
    //      bool TryUnwrapError(out TError e)

    public static Fallible<TValue, TError> WrapValue(TValue value)
    {
        return new Fallible<TValue, TError>(value);
    }

    public static Fallible<TValue, TError> WrapError(TError error)
    {
        return new Fallible<TValue, TError>(error);
    }


    /// enables implicit casting of a <typeparamref name="TValue"/> object
    /// into a <see cref="Fallible{T1, T2}"/>
    public static implicit operator Fallible<TValue, TError> (TValue value) => WrapValue(value);
    
    /// enables implicit casting of a <typeparamref name="TError"/> object
    /// into a <see cref="Fallible{T1, T2}"/>
    public static implicit operator Fallible<TValue, TError> (TError error) => WrapError(error);
}
