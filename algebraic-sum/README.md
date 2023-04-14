# Algebraic sum

Implements a concept known under the names of algebraic sum, tagged union, discriminated union and 
a few more. This concept is present in many popular languages e.g. Kotlin, Type Script, Haskell, F#.

It allows to define a type which can be one of many types (a sum of types in type theory parlance). 
For example, it could hold either an error or a successful value. Additionally, it works correctly 
with async/await.


## Basic `Fallible` example

The `Fallible<TSuccess, TError>` type an be used to remove some boilerplate and make 
code more concise.

Let's first imagine a method without using `Fallible`:
```csharp
public ViewAction ActionMethod(InputModel inputModdel)
{
    var validationResult = new Validator().Validate(inputmodel);
    if (!validationResult.IsValid)
    {
        CopyErrorsIntoContext(validationResult.Errors);
        return View();
    }

    var (outputModel, errors) = SomeProcessing(inputModel);
    if (errors != null)
    {
        CopyErrorsIntoContext(errors);
        return View();
    }

    return View(outputModel);
}

```

With `Fallible` it could become the following:
```csharp
public ViewAction ActionMethod(InputModel inputModdel)
{
    return new Validator().ValidateAsFallible(inputmodel)
        .Then(SomeProcessing)
        .Then(View) // the result of the happy path
        .DoWithError(CopyErrorsIntoContext)
        .OnError(_ => View()) // the result of the unhappy path
        .Unwrap(); // extract the content of the Fallible
}
```

The difference:
- we handle the whole of the happy path first
- we avoid duplicating the error handling blocks
- the way to signal errors is standardised
- the intent is more visible (but this may be subjective)


## `Fallible` with asynchronous methods

Asynchronous methods can be swapped in easily. The only consequence is that that the result of 
`.Unwrap()` is a `Task<T>` instead of `T`.
```csharp
public async Task<ViewAction> ActionMethod(InputModel inputModdel)
{
    return await new Validator().ValidateAsFallible(inputmodel)
        .Then(SomeProcessingAsync)
        .Then(View)
        .DoWithError(CopyErrorsIntoContext)
        .OnError(_ => View())
        .Unwrap();
}
```