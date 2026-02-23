// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Collections.Concurrent;
using System.Text.Json;
using ConsoleApp;

Console.WriteLine("Hello, World!");

var postcodeEnumerator = new PostcodeEnumerator();
IEnumerable<string> postcodes = postcodeEnumerator.EnumeratePostcodes("IM10AA")
        .Take(1)
    ;

string baseUrl = "https://services.gov.im/service/PrepaidPrescriptionCertificates/address-lookup/get/";

using var httpClient = new HttpClient();

JsonSerializerOptions jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};

int successCount = 0;
ConcurrentBag<string> failedPostcodes = new ConcurrentBag<string>();
string lastTriedPostcode = "-";
object consoleLock = new object();

Stopwatch stopwatch = Stopwatch.StartNew();
try
{
    await Parallel.ForEachAsync(
        postcodes,
        new ParallelOptions { MaxDegreeOfParallelism = 500 },
        async (postcode, cancellationToken) =>
        {
            Volatile.Write(ref lastTriedPostcode, postcode);
            string url = $"{baseUrl}{Uri.EscapeDataString(postcode)}?isAjaxRequest=true";
            using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
            string body = await response.Content.ReadAsStringAsync(cancellationToken);
            AddressLookupResponse? addressLookupResponse =
                JsonSerializer.Deserialize<AddressLookupResponse>(body, jsonOptions);
            if (!response.IsSuccessStatusCode || addressLookupResponse?.HasErrors != false)
            {
                failedPostcodes.Add(postcode);
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
}
catch (Exception e)
{
    Console.WriteLine($"Something failed during the postcode enumeration: {e}");
}

Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");
Console.WriteLine($"{successCount} successful postcodes");
Console.WriteLine($"{failedPostcodes.Count} failed postcodes \n{string.Join(", ", failedPostcodes)}");
Console.WriteLine($"Last tried postcode: {Volatile.Read(ref lastTriedPostcode)}");