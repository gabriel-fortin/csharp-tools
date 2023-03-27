
public record ValidationResult(bool IsValid, List<Error> Errors);

public record InputModel(string SomeData);

public record OutputModel(string Summary);

public record ViewAction(string viewName, object? viewModel);
