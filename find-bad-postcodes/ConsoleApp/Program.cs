// See https://aka.ms/new-console-template for more information

using ConsoleApp;

Console.WriteLine("Hello, World!");

var postcodeEnumerator = new PostcodeEnumerator();
IEnumerable<string> postcodes = postcodeEnumerator.EnumeratePostcodes("IM11AY")
    .Take(6);

foreach (string postcode in postcodes)
{
    Console.Write(postcode + "  ");
}