using MongoDB.Driver;
using MongoDB.Bson;

const string connectionString = "mongodb://localhost:27017";
var client = new MongoClient(connectionString);
var databases = client.ListDatabaseNames().ToList();
var dbList = string.Join(", ", databases);
Console.WriteLine("A: =======================");
Console.WriteLine("Database list:");
Console.WriteLine($"{dbList}");

Console.WriteLine("B: =======================");

var collections = client.GetDatabase("M165");
Console.WriteLine("Collections:");
if (collections == null)
    return;
foreach (var col in collections.ListCollections().ToList())
{
    Console.WriteLine($"{col["name"]}");
}
Console.WriteLine("C: ======================");

Console.WriteLine("First Film in year 2012:");

var moviesCollection = collections.GetCollection<Movies>("Movies");

Console.WriteLine($"{moviesCollection.Find(c => c.Year == 2012).FirstOrDefault().Title}");

Console.WriteLine("D: ======================");

Console.WriteLine("Films with Pierce Brosnan");
var res = moviesCollection.Find(c => c.Actors.Contains("Pierce Brosnan")).ToList();
foreach (var film in res)
{
    Console.WriteLine($"- {film.Title}");
}