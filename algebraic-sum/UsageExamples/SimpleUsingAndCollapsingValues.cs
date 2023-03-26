using AlgebraicSum;

public static class SimpleUsingAndCollapsingValues
{
    public static void Run()
    {
        Sum<string, int, Error> x1 = generateString();
        Sum<string, int, Error> x2 = generateInt();
        Sum<string, int, Error> x3 = generateError();
        Sum<string, int, Error> x = x1;

        x.Use(
            s => Console.WriteLine($"We've got a STRING: {s}"),
            i => Console.WriteLine($"We've got an INT: {i}"),
            err => Console.WriteLine($"Error ({err.Code}): {err.Message}")
        );

        string result = x.Reduce(
            s => $"We've got a STRING: {s}",
            i => $"We've got an INT: {i}",
            err => $"Error ({err.Code}): {err.Message}"
        );
        Console.WriteLine($"Result: {result}");
    }

    static string generateString() => "hi";

    static int generateInt() => 14;

    static Error generateError() => new Error(14, "oh no!");
}
