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
            // if the above succeeded, do some processing that transforms input model into output model (maybe an API call)
            .Then(SomeProcessing)
            // the above returns a Fallible<OutputModel, ErrorList>
            // if the above succeeded, use output model for a side effect but don't change it
            .Do(Audit)
            // the above returns a Fallible<OutputModel, ErrorList>
            // if the above succeeded, transform output model into a ViewAction
            .Then(View)
            // the above returns a Fallible<ViewAction, ErrorList>
            // if the above failed, use error list for a side effect but don't change it
            .DoOnError(CopyErrorsToModelState)
            // the above returns a Fallible<ViewAction, ErrorList>
            // if the above failed, transform error list into a View Action
            .OnError(View)
            // the above returns a Fallible<ViewAction, ViewAction>
            // transform into a plain ViewAction
            .Unwrap();
    }

}