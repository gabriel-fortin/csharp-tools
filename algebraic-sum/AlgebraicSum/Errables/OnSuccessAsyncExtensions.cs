namespace org.g14.AlgebraicSum.Errables;

public static class OnSuccessAsyncExtensions
{
    // Let's handle a few variations of the OnSuccessAsync invocation. Variations include:
    //  - receiver object sync/async: Errable<...> or Task<Errable<...>>
    //  - transform's result sync/async: Errable<...> or Task<Errable<...>>
    //  - transform's result wrapped/plain: Errable<TResult,...> or TResult
    // The transforming function is called a mapper if it returns a plain result

    // sync receiver, async transform, wrapped transform's result
    public static async Task<Errable<TNextValue, TError>> OnSuccessAsync<TValue, TError, TNextValue>(
        this Errable<TValue, TError> errable,
        Func<TValue, Task<Errable<TNextValue, TError>>> transformAsync)
    {
        return await errable.Reduce(
            async value => await transformAsync(value),
            error => Task.FromResult(new Errable<TNextValue, TError>(error))
        );
    }

    // async receiver, async transform, wrapped transform's result
    public static async Task<Errable<TNextValue, TError>> OnSuccessAsync<TValue, TError, TNextValue>(
        this Task<Errable<TValue, TError>> errableTask,
        Func<TValue, Task<Errable<TNextValue, TError>>> transformAsync)
    {
        return await OnSuccessAsync(await errableTask, transformAsync);
    }

    // sync receiver, sync transform, wrapped transform's result - exists in Errable class
    // sync receiver, sync transform, plain transform's result - exists in Errable class

    // async receiver, sync transform, wrapped transform's result
    public static async Task<Errable<TNextValue, TError>> OnSuccessAsync<TValue, TError, TNextValue>(
        this Task<Errable<TValue, TError>> errableTask,
        Func<TValue, Errable<TNextValue, TError>> transform)
    {
        return (await errableTask).OnSuccess(transform);
    }

    // async receiver, sync transform, plain mapper's result
    public static async Task<Errable<TNextValue, TError>> OnSuccessAsync<TValue, TError, TNextValue>(
        this Task<Errable<TValue, TError>> errableTask,
        Func<TValue, TNextValue> mapper)
    {
        return (await errableTask).OnSuccess(mapper);
    }

    // async receiver, async transform, plain transform's result - assumed not needed
    // sync receiver, async transform, plain transform's result - assumed not needed
}
