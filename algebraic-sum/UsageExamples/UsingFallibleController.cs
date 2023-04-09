using AlgebraicSum.Fallibles;

public class UsingFallibleController : SimplifiedController
{
    // practically, we'll probably have some type to represent errors, here's some imitation
    public class ErrorList : List<Error> {}

    // practically, we'll probably have some processing to do, here's an imitation
    public Fallible<OutputModel, ErrorList> SomeProcessing(InputModel inputModel)
    {
        // implicit conversion to a Fallible
        return new OutputModel(inputModel.SomeData);
    }

    // here's another processing method but async this time
    public Task<Fallible<OutputModel, ErrorList>> SomeProcessingAsync(InputModel inputModel)
    {
        Fallible<OutputModel, ErrorList> result = new OutputModel(inputModel.SomeData); // implicit cast
        return Task.FromResult(result);
    }

    // let's say we'll want the output data to be audited
    // note that this method is void-returning
    public void Audit(OutputModel outputModel)
    {
        Console.WriteLine($"auditing: {outputModel.Summary}");
    }

    // in case of errors, we'll want to transform errors to model state errors
    // this method is also void-returning
    public void CopyErrorsToModelState(ErrorList errors)
    {
        ModelStateErrors.AddRange(errors.Select(x => x.Message));
    }



    public ViewAction ActionMethod(InputModel inputModel)
    {
        return new Validator().ValidateAsFallible(inputModel)
            // the above returns a Fallible<InputModel, ErrorList>
            // if the above succeeded, do some processing that transforms input model into output model
            .Then(SomeProcessing)
            // the above returns a Fallible<OutputModel, ErrorList>
            // if the above succeeded, use output model for a side effect (without changing it)
            .Do(Audit)
            // the above returns a Fallible<OutputModel, ErrorList>
            // if the above succeeded, transform output model into a ViewAction using `View(object)`
            .Then(View)
            // the above returns a Fallible<ViewAction, ErrorList>
            // if the above failed, use the error list for a side effect (without changing it)
            .DoWithError(CopyErrorsToModelState)
            // the above returns a Fallible<ViewAction, ErrorList>
            // if the object contains errors, transform the error list into a View Action
            .OnError(_ => View())
            // the above returns a Fallible<ViewAction, ViewAction>
            // extract the value (be it success or error, both are a ViewAction)
            .Unwrap();
            // the above returns a ViewAction
    }

    public async Task<ViewAction> ActionMethodAsync(InputModel inputModel)
    {
        return await new Validator().ValidateAsFallibleAsync(inputModel)
            // the above returns a Task<Fallible<InputModel, ErrorList>>
            // if the above succeeded, use its result to do some async processing (maybe an API call)
            .Then(SomeProcessingAsync)
            // the above returns a Task<Fallible<OutputModel, ErrorList>>
            // if the above succeeded, use output model for a side effect (without changing it)
            .Do(Audit)
            // the above returns a Task<Fallible<OutputModel, ErrorList>>
            // if the above succeeded, transform output model into a ViewAction
            .Then(View)
            // the above returns a Task<Fallible<ViewAction, ErrorList>>
            // if the above failed, use the error list for a side effect (without changing it)
            .DoWithError(CopyErrorsToModelState)
            // the above returns a Task<Fallible<ViewAction, ErrorList>>
            // if the object contains errors, transform the error list into a View Action
            .OnError(_ => View())
            // the above returns a Task<Fallible<ViewAction, ViewAction>>
            // extract the value (be it success or error, both are a ViewAction at this point)
            .Unwrap();
            // the above returns a Task<ViewAction>
    }

    // public ViewAction ActionMethod_v1(InputModel inputModel)
    // {
    //     return new Validator().ValidateAsFallible(inputModel)
    //         .Then(SomeProcessing)
    //         .Do(Audit)
    //         .ThenMap(View)
    //         .DoOnError(CopyErrorsToModelState)
    //         .OnError(View)
    //         .Unwrap();
    // }

    // public Task<ViewAction> ActionMethodAsync_v1(InputModel inputModel)
    // {
    //     return new Validator().ValidateAsFallibleAsync(inputModel)
    //         .Then(SomeProcessingAsync)
    //         .Do(Audit)
    //         .ThenMap(View)
    //         .DoOnError(CopyErrorsToModelState)
    //         .OnError(View)
    //         .Unwrap();
    // }

}
