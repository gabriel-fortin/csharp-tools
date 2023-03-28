namespace AlgebraicSum;

public static class Extensions
{
    public static T Reduce<T>(this Sum<T, T, T> errable)
        => errable.Reduce(x => x, x => x, x => x);

    public static T Reduce<T>(this Sum<T, T> errable)
        => errable.Reduce(x => x, x => x);
}