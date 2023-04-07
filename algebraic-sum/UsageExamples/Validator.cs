using AlgebraicSum.Errables;
using AlgebraicSum.Fallibles;

public class Validator
{
    public ValidationResult Validate(InputModel model)
    {
        if (model.SomeData.Length < 5)
        {
            return new ValidationResult(IsValid: false, new List<Error> { new Error(7, "too short") });
        }
        return new ValidationResult(IsValid: true, new List<Error>());
    }

    public Errable<InputModel, List<Error>> ValidateIntoErrable(InputModel model)
    {
        ValidationResult validationResult = Validate(model);
        if (validationResult.IsValid)
        {
            return new Errable<InputModel, List<Error>>(model);
        }
        return new Errable<InputModel, List<Error>>(validationResult.Errors);
    }

    public Fallible<InputModel, UsingFallibleController.ErrorList> ValidateAsFallible(InputModel model)
    {
        ValidationResult validationResult = Validate(model);
        if (validationResult.IsValid)
        {
            return Fallible<InputModel, UsingFallibleController.ErrorList>.WrapValue(model);
        }
        var errorList = new UsingFallibleController.ErrorList();
        errorList.AddRange(validationResult.Errors);
        return Fallible<InputModel, UsingFallibleController.ErrorList>.WrapError(errorList);
    }

    public Task<Fallible<InputModel, UsingFallibleController.ErrorList>> ValidateAsFallibleAsync(InputModel model)
    {
        return Task.FromResult(ValidateAsFallible(model));
    }
}

