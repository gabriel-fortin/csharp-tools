// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Collections.Concurrent;
using ConsoleApp;

Console.WriteLine("Hello, World!");

var postcodeEnumerator = new PostcodeEnumerator();
IEnumerable<string> postcodes = postcodeEnumerator.EnumeratePostcodes("IM10AA")
    .Take(1)
    ;

string baseUrl = "https://services.gov.im/service/DriversAndVehicles/Vehicle/VehicleLicenceDuplicates/AddressLookup?isAjaxRequest=true";

using var httpClient = new HttpClient();

int successCount = 0;
ConcurrentBag<string> failedPostcodes = new ConcurrentBag<string>();
string lastTriedPostcode = "-";
object consoleLock = new object();

Stopwatch stopwatch = Stopwatch.StartNew();
await Parallel.ForEachAsync(
    postcodes,
    new ParallelOptions { MaxDegreeOfParallelism = 500 },
    async (postcode, cancellationToken) =>
    {
        Volatile.Write(ref lastTriedPostcode, postcode);
        string url = $"{baseUrl}&postcode={Uri.EscapeDataString(postcode)}";
        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            failedPostcodes.Add(postcode);
            string body = await response.Content.ReadAsStringAsync(cancellationToken);
            lock (consoleLock)
            {
                Console.WriteLine($"\n\n\n{postcode} - {response.StatusCode} {response.ReasonPhrase}\n{body}");
            }
        }
        else
        {
            Interlocked.Increment(ref successCount);
        }
    });

Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");
Console.WriteLine($"{successCount} successful postcodes");
Console.WriteLine($"{failedPostcodes.Count} failed postcodes \n{string.Join(", ", failedPostcodes)}");
Console.WriteLine($"Last tried postcode: {Volatile.Read(ref lastTriedPostcode)}");
