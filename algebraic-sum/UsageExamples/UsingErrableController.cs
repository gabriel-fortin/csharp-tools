using AlgebraicSum;

public class UsingErrableController : SimplifiedController
{
    public Errable<OutputModel, List<Error>> SomeProcessing(InputModel model)
    {
        return new OutputModel(new string(model.SomeData.Reverse().ToArray()));
    }

    public ViewAction Action_DesiredLookLike(InputModel viewModel)
    {
        // We'd like something that looks like this
        // return new Validator().Validate(viewModel)
        //     .OnSuccess(SomeProcessing)
        //     .OnSuccess(View)
        //     .OnError(AddErrorsToModelState.AndThen(View));
            // or maybe
            // .OnError(AddErrorsToModelStateAndReturnView);
            throw new NotImplementedException();
    }

    // let's use what we have as is

    public ViewAction Action1(InputModel viewModel)
    {
        // return Errable.FromValue<List<Error>, InputModel>(viewModel)
        return new Errable<InputModel, List<Error>>(viewModel)
            .OnSuccess(x =>
            {
                ValidationResult validationResult = new Validator().Validate(x);
                return validationResult.IsValid
                    ? new Errable<InputModel, List<Error>>(x)
                    : new Errable<InputModel, List<Error>>(validationResult.Errors);
            })
            .OnSuccess(SomeProcessing)
            .Reduce(
                outputViewModel => View(outputViewModel),
                errors =>
                {
                    ModelStateErrors = errors.Select(x => x.Message).ToList();
                    return View();
                }
            );
    }

    // let's replace Reduce with OnSuccess + OnError

    public ViewAction Action2(InputModel viewModel)
    {
        // return Errable.FromValue<InputModel, List<Error>>(viewModel)
        return new Errable<InputModel, List<Error>>(viewModel)
            .OnSuccess(x =>
            {
                ValidationResult validationResult = new Validator().Validate(x);
                return validationResult.IsValid
                    ? new Errable<InputModel, List<Error>>(x)
                    : new Errable<InputModel, List<Error>>(validationResult.Errors);
            })
            .OnSuccess(SomeProcessing)
            .OnSuccess(outputViewModel => View(outputViewModel))           //  <====
            .OnError(errors =>                                             //  <====
            {                                                              //  <====
                ModelStateErrors = errors.Select(x => x.Message).ToList(); //  <====
                return View();                                             //  <====
            })                                                             //  <====
            .Reduce(); // and we have to call a version of Reduce anyway   //  <====
    }

    // after adding a method to the Validator which returns an Errable

    public ViewAction Action3(InputModel viewModel)
    {
        return new Validator().ValidateIntoErrable(viewModel)             //  <====
            .OnSuccess(SomeProcessing)
            .OnSuccess(outputViewModel => View(outputViewModel))
            .OnError(errors =>
            {
                ModelStateErrors = errors.Select(x => x.Message).ToList();
                return View();
            })
            .Reduce();
    }

    // after adding a Controller's extension to copy errors into State Model and return a View

    public ViewAction Action4(InputModel viewModel)
    {
        return new Validator().ValidateIntoErrable(viewModel)
            .OnSuccess(SomeProcessing)
            .OnSuccess(outputViewModel => View(outputViewModel))
            .OnError(this.CopyErrorsToModelStateAndReturn(() => View()))  //  <====
            .Reduce();
    }

    // simplify: replace lambdas with method reference where possible

    public ViewAction Action5(InputModel viewModel)
    {
        return new Validator().ValidateIntoErrable(viewModel)
            .OnSuccess(SomeProcessing)
            .OnSuccess(View)                                       //  <====
            .OnError(this.CopyErrorsToModelStateAndReturn(View))   //  <====
            .Reduce();
    }

    // maybe we want to go back to a two-parameter Reduce?

    public ViewAction Action6(InputModel viewModel)
    {
        return new Validator().ValidateIntoErrable(viewModel)
            .OnSuccess(SomeProcessing)
            .Reduce(                                                 // <====
                onSuccess: View,                                     // <====
                onError: this.CopyErrorsToModelStateAndReturn(View)  // <====
            );
    }

    public ViewAction Action_Classic_O_O_P(InputModel viewModel)
    {
        ValidationResult validationResult = new Validator().Validate(viewModel);
        if (!validationResult.IsValid)
        {
            this.CopyErrorsToModelState(validationResult);
            return View();
        }
        Errable<OutputModel, List<Error>> result = SomeProcessing(viewModel);
        if (result.TryGetT2(out var errors))
        {
            this.CopyErrorsToModelState(validationResult);
            return View();
        }
        return View();
    }

}

public static class ControllerExtensions
{
    public static Func<List<Error>, ViewAction> CopyErrorsToModelStateAndReturn(
        this SimplifiedController controller, Func<ViewAction> renderFunc)
    {
        return (errors) => {
            controller.ModelStateErrors = errors.Select(x => x.Message).ToList();
            return renderFunc();
        };
    }

    public static void CopyErrorsToModelState(this SimplifiedController controller,
        ValidationResult validationResult)
    {
        controller.ModelStateErrors = validationResult.Errors.Select(x => x.Message).ToList();
    }
}
