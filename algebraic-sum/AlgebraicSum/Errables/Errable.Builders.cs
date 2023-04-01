namespace AlgebraicSum.Errables;

public static class Errable
{
    public static Errable<TValue, TError> FromValue<TValue, TError>(TValue value)
        => new Errable<TValue, TError>(value);

    public static Errable<TValue, TError> FromError<TValue, TError>(TError errorValue)
        => new Errable<TValue, TError>(errorValue);
}
