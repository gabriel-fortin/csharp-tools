namespace org.g14.AlgebraicSum.Errables;

public static class ReduceAsyncExtensions
{
    // Let's handle a few variations of the ReduceAsync invocation. Variations include:
    //  - receiver object sync/async: Errable<...> or Task<Errable<...>>
    //  - transform's result sync/async: Errable<...> or Task<Errable<...>>
    // The transforming function is called a mapper if it returns a plain result

    // async receiver, sync both onSuccess, onError transforms
    public static async Task<TResult> ReduceAsync<TValue, TError, TResult>(
        this Task<Errable<TValue, TError>> errableTask,
        Func<TValue, TResult> onSuccess,
        Func<TError, TResult> onError)
    {
        return (await errableTask).Reduce(onSuccess, onError);
    }

    // async receiver, sync just onError transform
    public static async Task<T> ReduceAsync<T, TError>(
        this Task<Errable<T, TError>> errableTask,
        Func<TError, T> onError)
    {
        return (await errableTask).Reduce(x => x, onError);
    }

    // async receiver, no transforms
    public static async Task<T> ReduceAsync<T>(this Task<Errable<T, T>> errableTask)
    {
        return (await errableTask).Reduce(x => x, x => x);
    }

    // async receiver, async transforms (either one or two) - not implemented yet

    // sync receiver, sync transforms (either one or two) - covered in Errable
    // sync receiver, no transforms - covered in ReduceExtensions
}
