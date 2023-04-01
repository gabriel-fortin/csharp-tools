namespace AlgebraicSum.Errables;

public static class OnErrorAsyncExtensions
{
    // Let's handle a few variations of the OnErrorAsync invocation. Variations include:
    //  - receiver object sync/async: Errable<...> or Task<Errable<...>>
    //  - transform's result sync/async: Errable<...> or Task<Errable<...>>
    //  - transform's result wrapped/plain: Errable<TResult,...> or TResult
    // The transforming function is called a mapper if it returns a plain result

    // async receiver, sync mapper, plain mapper's result
    public static async Task<Errable<TValue, TNextError>> OnErrorAsync<TValue, TError, TNextError>(
        this Task<Errable<TValue, TError>> errableTask,
        Func<TError, TNextError> mapper)
        {
            return (await errableTask).OnError(mapper);
        }
    
    // async receiver, sync mapper, wrapped mapper's result - not implemented yet
    // async receiver, async mapper, plain mapper's result - not implemented yet
    // async receiver, async mapper, wrapped mapper's result - not implemented yet

    // sync receiver, sync mapper, plain mapper's result - covered in Errable
    // sync receiver, sync mapper, wrapped mapper's result - covered in Errable
    // sync receiver, async mapper, plain mapper's result - not implemented yet
    // sync receiver, async mapper, wrapped mapper's result - not implemented yet
}
