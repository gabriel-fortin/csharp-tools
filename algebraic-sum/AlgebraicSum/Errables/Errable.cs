namespace org.g14.AlgebraicSum.Errables;

// alternative name:
// Fallible<TError, TValue>

public class Errable<TValue, TError> : Sum<TValue, TError>
{
    public Errable(TValue value) : base(value) { }

    public Errable(TError error) : base(error) { }


    /// <summary>
    /// Executes the given transform if this object holds a successful value
    /// </summary>
    /// <remark> This would be called 'bind' in monad's world </remark>
    public Errable<TNextValue, TError> Then<TNextValue>(Func<TValue, Errable<TNextValue, TError>> transform)
    {
        return OnSuccess(transform);
    }

    /// <summary>
    /// Executes the given transform if this object holds a successful value
    /// </summary>
    public Errable<TNextValue, TError> Then<TNextValue>(Func<TValue, TNextValue> transform)
    {
        return OnSuccess(transform);
    }

    /// <summary>
    /// Executes the given transform if this object holds a successful value
    /// </summary>
    /// <remark> This would be called 'bind' in monad's world </remark>
    public Errable<TNextValue, TError> OnSuccess<TNextValue>(
        Func<TValue, Errable<TNextValue, TError>> transform)
    {
        return Reduce(
            value => transform(value),
            error => new Errable<TNextValue, TError>(error)
        );
    }

    /// <summary>
    /// Executes the given transform if this object holds a successful value
    /// </summary>
    /// <remark>
    /// In this overload the transform is assumed to never fail (because it doesn't return an Errable)
    /// </remark>
    public Errable<TNextValue, TError> OnSuccess<TNextValue>(
        Func<TValue, TNextValue> transform)
    {
        return Reduce(
            value => new Errable<TNextValue, TError>(transform(value)),
            error => new Errable<TNextValue, TError>(error)
        );
    }

    /// <summary>
    /// Executes the given transform if this object holds an error value
    /// </summary>
    public Errable<TValue, TNextError> OnError<TNextError>(
        Func<TError, Errable<TValue, TNextError>> transform)
    {
        return Reduce(
            value => new Errable<TValue, TNextError>(value),
            error => transform(error)
        );
    }

    /// <summary>
    /// Executes the given transform if this object holds an error value
    /// </summary>
    /// <remark>
    /// In this overload the transform is assumed to never fail (because it doesn't return an Errable)
    /// </remark>
    public Errable<TValue, TNextError> OnError<TNextError>(Func<TError, TNextError> transform)
    {
        return Reduce(
            value => new Errable<TValue, TNextError>(value),
            error => new Errable<TValue, TNextError>(transform(error))
        );
    }

    /// <summary>
    /// Collapses this object into a single value. Executes one of the given mappers depending
    /// on the content of this <see cref="Errable{T1, T2}" /> and returns the result from that mapper.
    /// </summary>
    /// <remark>Both mappers must have the same return type</remark>
    public new TResult Reduce<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onError)
    {
        return base.Reduce(onSuccess, onError);
    }

    /// <summary>
    /// Collapses this object into a single value. If this <see cref="Errable{T1, T2}" /> contains
    /// a success value, that value is returned. If it contains an error, the given
    /// <paramref name="onError"/> mapper is executed and its result returned.
    /// </summary>
    /// <remark>The <paramref name="onError"/> mapper must return a <typeparamref name="TValue"/></remark>
    public TValue Reduce(Func<TError, TValue> onError)
    {
        return Reduce(x => x, onError);
    }


    public static Errable<TValue, TError> FromValue(TValue value)
        => new Errable<TValue, TError>(value);

    public static Errable<TValue, TError> FromError(TError errorValue)
        => new Errable<TValue, TError>(errorValue);


    public static implicit operator Errable<TValue, TError> (TValue val)
        => new Errable<TValue, TError>(val);

    public static implicit operator Errable<TValue, TError> (TError err)
        => new Errable<TValue, TError>(err);
}
