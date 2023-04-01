namespace AlgebraicSum.Errables;

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

    public new TResult Reduce<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onError)
    {
        return base.Reduce(onSuccess, onError);
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
