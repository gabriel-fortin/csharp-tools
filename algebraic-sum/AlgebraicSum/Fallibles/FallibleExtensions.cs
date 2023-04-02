namespace AlgebraicSum.Fallibles;

public static class FallibleExtensions
{
    /// <summary>
    /// Extracts the content of this <see cref="Fallible{TValue, TError}"/>
    /// when the types for value and error are the same
    /// </summary>
    public static T Unwrap<T>(this Fallible<T, T> fallible)
    {
        return fallible.Unwrap(x => x, x => x);
    }
}
