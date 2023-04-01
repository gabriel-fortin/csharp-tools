using AlgebraicSum.Errables;

public class UsingAsyncErrableController : SimplifiedController
{
    public Task<Errable<OutputModel, List<Error>>> SomeProcessingAsync(InputModel model)
    {
        return Task.FromResult(
            Errable.FromValue<OutputModel, List<Error>>(
                new OutputModel(new string(model.SomeData.Reverse().ToArray()))));
    }

    public Task<ViewAction> Action_DesiredLookLike(InputModel viewModel)
    {
        // // We'd like something that looks like this
        // return await new AsyncErrable<InputModel, List<Error>>(viewModel)
        //     .OnSuccess(new Validator().ValidateIntoErrable)
        //     .OnSuccess(SomeProcessingAsync)
        //     .OnSuccess(View)
        //     .OnError(this.CopyErrorsToModelStateAndReturn(View));

        // // or like this
        // return new Validator().ValidateIntoErrable(viewModel)
        //     .OnSuccess(SomeProcessingAsync)
        //     .OnSuccess(View)
        //     .OnError(this.CopyErrorsToModelStateAndReturn(View));

        throw new NotImplementedException();
    }

    // let's return a Task<Errable<...>> on every step

    public async Task<ViewAction> Action1(InputModel viewModel)
    {
        return await Task.FromResult(new Validator().ValidateIntoErrable(viewModel))
            .OnSuccessAsync(SomeProcessingAsync)
            .ReduceAsync(
                onSuccess: value => View(),
                onError: error => this.CopyErrorsToModelStateAndReturn(View)(error)
            );
    }

    // let's shorten the Reduce

    public async Task<ViewAction> Action3(InputModel viewModel)
    {
        return await Task.FromResult(new Validator().ValidateIntoErrable(viewModel))
            .OnSuccessAsync(SomeProcessingAsync)
            .ReduceAsync(
                onSuccess: View,
                onError: this.CopyErrorsToModelStateAndReturn(View)
            );
    }

    // let's simplify the first call

    public async Task<ViewAction> Action4(InputModel viewModel)
    {
        return await new Validator().ValidateIntoErrable(viewModel)
            .OnSuccessAsync(SomeProcessingAsync)
            .ReduceAsync(
                onSuccess: View,
                onError: this.CopyErrorsToModelStateAndReturn(View)
            );
    }

    // let's replace the Reduce

    public async Task<ViewAction> Action6(InputModel viewModel)
    {
        return await new Validator().ValidateIntoErrable(viewModel)
            .OnSuccessAsync(SomeProcessingAsync)
            .OnSuccessAsync(View)
            .OnErrorAsync(this.CopyErrorsToModelStateAndReturn(View))
            .ReduceAsync();
    }

    // with a one-argument version of ReduceAsync

    public async Task<ViewAction> Action7(InputModel viewModel)
    {
        return await new Validator().ValidateIntoErrable(viewModel)
            .OnSuccessAsync(SomeProcessingAsync)
            .OnSuccessAsync(View)
            .ReduceAsync(onError: this.CopyErrorsToModelStateAndReturn(View));
    }
}
