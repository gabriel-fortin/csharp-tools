// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using ConsoleApp;

Console.WriteLine("Hello, World!");

var postcodeEnumerator = new PostcodeEnumerator();
IEnumerable<string> postcodes = postcodeEnumerator.EnumeratePostcodes("IM14GV")
    .Take(1000);

string baseUrl = "https://services.gov.im/service/DriversAndVehicles/Vehicle/VehicleLicenceDuplicates/AddressLookup?isAjaxRequest=true";

using var httpClient = new HttpClient();

int successCount = 0;
List<string> failedPostcodes = new List<string>();
string lastTriedPostcode = "-";

Stopwatch stopwatch = Stopwatch.StartNew();
foreach (string postcode in postcodes)
{
    lastTriedPostcode = postcode;
    string url = $"{baseUrl}&postcode={Uri.EscapeDataString(postcode)}";
    using HttpResponseMessage response = await httpClient.GetAsync(url);
    if (!response.IsSuccessStatusCode)
    {
        failedPostcodes.Add(postcode);
        Console.WriteLine($"\n\n\n{postcode} - {response.StatusCode} {response.ReasonPhrase}\n{await response.Content.ReadAsStringAsync()}");
    }
    else
    {
        successCount++;
    }
}

Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");
Console.WriteLine($"{successCount} successful postcodes");
Console.WriteLine($"{failedPostcodes.Count} failed postcodes \n{string.Join(", ", failedPostcodes)}");
Console.WriteLine($"Last tried postcode: {lastTriedPostcode}");