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
    public Fallible(TValue value)
    {
        this.internalValue = value;
    }

    /// <summary>
    /// Constructs a <see cref="Fallible{T1, T2}"/> containing an erroneous value
    /// </summary>
    public Fallible(TError error)
    {
        this.internalValue = error;
    }

    /// <summary>
    /// If this object contains a successful value, runs the given <paramref name="mapper"/> and
    /// returns a new <see cref="Fallible{T1, T2}"/> using the <paramref name="mapper"/>'s return
    /// value as the new successful value.
    /// Otherwise, returns a new <see cref="Fallible{T1, T2}"/> using the same erroneous
    /// value as this object.
    /// </summary>
    public Fallible<TNextValue, TError> Then<TNextValue>(Func<TValue, TNextValue> mapper)
    {
        return internalValue.Reduce<Fallible<TNextValue, TError>>(
            value => mapper(value),
            errors => errors
        );
    }

    /// <summary>
    /// If this object contains a successful value, runs the given <paramref name="fallibleMapper"/>
    /// and returns its return value.
    /// Otherwise, returns a new <see cref="Fallible{T1, T2}"/> using the same erroneous
    /// value as this object.
    /// </summary>
    public Fallible<TNextValue, TError> Then<TNextValue>(Func<TValue, Fallible<TNextValue, TError>> fallibleMapper)
    {
        return internalValue.Reduce<Fallible<TNextValue, TError>>(
            value => fallibleMapper(value),
            errors => errors
        );
    }

    /// <summary>
    /// If this object contains an erroneous value, runs the given <paramref name="mapper"/> and
    /// returns a new <see cref="Fallible{T1, T2}"/> using the <paramref name="mapper"/>'s return
    /// value as the new erroneous value.
    /// Otherwise, returns a new <see cref="Fallible{T1, T2}"/> using the same successful
    /// value as this object.
    /// </summary>
    public Fallible<TValue, TNextError> OnError<TNextError>(Func<TError, TNextError> mapper)
    {
        return internalValue.Reduce<Fallible<TValue, TNextError>>(
            value => value,
            errors => mapper(errors)
        );
    }

    /// <summary>
    /// If this object contains an erroneous value, runs the given <paramref name="fallibleMapper"/>
    /// and returns its return value.
    /// Otherwise, returns a new <see cref="Fallible{T1, T2}"/> using the same successful
    /// value as this object.
    /// </summary>
    public Fallible<TValue, TNextError> OnError<TNextError>(Func<TError, Fallible<TValue, TNextError>> fallibleMapper)
    {
        return internalValue.Reduce<Fallible<TValue, TNextError>>(
            value => value,
            errors => fallibleMapper(errors)
        );
    }

    /// <summary>
    /// If this object contains a successful value, runs the given <paramref name="action"/>.
    /// Always returns itself.
    /// </summary>
    public Fallible<TValue, TError> Do(Action<TValue> action)
    {
        internalValue.Use(action, x => {});
        return this;
    }

    /// <summary>
    /// If this object contains an erroneous value, runs the given <paramref name="action"/>.
    /// Always returns itself.
    /// </summary>
    public Fallible<TValue, TError> DoOnError(Action<TError> action)
    {
        internalValue.Use(x => {}, action);
        return this;
    }

    /// <summary>
    /// Extracts the content of this <see cref="Fallible{TValue, TError}"/>
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


    /// enables implicit casting of a <typeparamref name="TValue"/> object into a <see cref="Fallible{T1, T2}"/>
    public static implicit operator Fallible<TValue, TError> (TValue value) => new Fallible<TValue, TError>(value);
    
    /// enables implicit casting of a <typeparamref name="TError"/> object into a <see cref="Fallible{T1, T2}"/>
    public static implicit operator Fallible<TValue, TError> (TError error) => new Fallible<TValue, TError>(error);
}
