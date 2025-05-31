using m165_15;
using MongoDB.Driver;
using MongoDB.Bson;

const string connectionString = "mongodb://localhost:27017";
MongoClient client = new MongoClient(connectionString);
try
{
    // Attempt to ping the MongoDB server
    var database = client.GetDatabase("admin");
    var command = new BsonDocument("ping", 1);
    database.RunCommandAsync<BsonDocument>(command).Wait();
    Console.WriteLine("MongoDB connection successful.");
}
catch (Exception ex)
{
    Console.WriteLine($"MongoDB connection failed: {ex.Message}");
    return;
}

var databases = client.ListDatabaseNames().ToList();
Console.WriteLine("A: =======================");
Console.WriteLine("Database list:");
foreach (var db in databases)
{
    Console.WriteLine($"\t- {db}");
}

Console.WriteLine("B: =======================");

var collections = client.GetDatabase("M165");
Console.WriteLine("Collections:");
if (collections == null)
    return;
foreach (var col in collections.ListCollections().ToList())
{
    Console.WriteLine($"\t- {col["name"]}");
}
Console.WriteLine("C: ======================");

Console.WriteLine("First Film in year 2012:");

var moviesCollection = collections.GetCollection<Movies>("Movies");
// C# LINQ syntax: (way fucking simpler)
//Console.WriteLine($"\t - {moviesCollection.Find(c => c.Year == 2012).FirstOrDefault().Title}");
var filterC = Builders<Movies>.Filter.Eq("Year", 2012);
Console.WriteLine($"-\t {moviesCollection.Find(filterC).FirstOrDefault().Title}");

Console.WriteLine("D: ======================");

Console.WriteLine("Films with Pierce Brosnan");
//C# LINQ syntax: (way fucking simpler)
//var res = moviesCollection.Find(c => c.Actors.Contains("Pierce Brosnan")).ToList();
var filterD = Builders<Movies>.Filter.In("Actors", "Pierce Brosnan");
var res = moviesCollection.Find(filterD).ToList();
foreach (var film in res)
{
    Console.WriteLine($"\t- {film.Title}");
}
Console.WriteLine("E: ======================");
Movies movie = new Movies
{
    Title = "The Da Vinci Code",
    Year = 2006,
    Summary = "So dunkel ist der Betrug an dder Menschheit",
    Actors = ["Tom Hanks", "Audrey Tautou"]
};
moviesCollection.InsertOne(movie);

Console.WriteLine($"Added movie: {movie.Title} ({movie.Year})");

Console.WriteLine("F: ======================");
List<Movies> moviesArray = [new()
{
    Title = "Ocean's Eleven",
    Year = 2001,
    Summary = "Bist du drin oder drraussen?",
    Actors = ["George Clooney", "Brad Pitt", "Julia Roberts"]
},new()
{
    Title = "The Davinci Code",
    Year = 2004,
    Summary = "Die Elf sind jetzt Zwölf",
    Actors = ["George Clooney", "Brad Pitt", "Julia Roberts", "Andy Garcia"]
}];
if(moviesCollection.Find(c => c.Title == "The Davinci Code").Count() == 0 && 
   moviesCollection.Find(c => c.Title == "Ocean's Eleven").Count() == 0)
{
    Console.WriteLine("No movies found, inserting new ones.");
    moviesCollection.InsertMany(moviesArray);
    Console.WriteLine($"Added {moviesArray.Count} movies:");
    // C# LINQ syntax: (way fucking simpler)
    //moviesArray = moviesCollection.Find(c => moviesArray.Select(m => m.Title).Contains(c.Title)).ToList();
    
    var filterF = Builders<Movies>.Filter.In("Title", moviesArray.Select(m => m.Title));
    moviesArray = moviesCollection.Find(filterF).ToList();
    foreach (var m in moviesArray)
    {
        Console.WriteLine($"\t- {movie.Title} ({movie.Id})");
    }
}
else 
{
    Console.WriteLine("Movies already exist, skipping insertion.");
}

Console.WriteLine("G: ======================");

Console.WriteLine("changing title of all Skyfall - 007 movies to Skyfall");
// C# LINQ syntax: (way fucking simpler)
/*
moviesCollection.AsQueryable()
    .Where(m => m.Title == "Skyfall - 007")
    .ToList()
    .ForEach(m => m.Title = "Skyfall");
*/

var updateDefinition = Builders<Movies>.Update.Set(m => m.Title, "Skyfall");
moviesCollection.UpdateMany(m => m.Title == "Skyfall - 007", updateDefinition);

Console.WriteLine("H: ======================");

Console.WriteLine("Deleting all movies year <= 1995");

// C# LINQ syntax: (way fucking simpler)
//moviesCollection.DeleteMany(m => m.Year <= 1995);
var filterH = Builders<Movies>.Filter.Lte("Year", 1995);
moviesCollection.DeleteMany(filterH);

Console.WriteLine("I: ======================");
Console.WriteLine("Films per year >= 2000:");

//C# LINQ syntax: (way fucking simpler)
/*
var filmsPerYear = moviesCollection.AsQueryable()
   .Where(m => m.Year >= 2000)
   .GroupBy(m => m.Year)
   .Select(g => new FilmsPerYear { Year = g.Key, Count = g.Count() })
   .OrderBy(f => f.Year)
   .ToList();
*/
var sort = Builders<FilmsPerYear>.Sort.Ascending("Year");
var filterI = Builders<Movies>.Filter.Gte("Year", 2000);
var filmsPerYear = moviesCollection.Aggregate()
    //.Match(m => m.Year >= 2000)
    .Match(filterI)
    .Group(m => m.Year, g => new FilmsPerYear(){ Year = g.Key, Count = g.Count() })
    .Sort(sort)
    .ToList();

foreach (var item in filmsPerYear)
{
    Console.WriteLine($"\t- {item.Year}: {item.Count} films");
}

Console.WriteLine("J: ======================");
Console.WriteLine("All movies in collection as JSON:");

// C# LINQ syntax: (way fucking simpler)
//foreach (var m in moviesCollection.Find(m => true).ToList().toJson())
foreach (var m in moviesCollection.Find(FilterDefinition<Movies>.Empty).ToList())
{
    Console.WriteLine($"\t- {m.ToJson()}");
}

Console.WriteLine("======================================================");

Console.WriteLine("All movies in the collection:");
// C# LINQ syntax: (way fucking simpler)
//foreach (var m in moviesCollection.Find(m => true).ToList())
foreach (var m in moviesCollection.Find(FilterDefinition<Movies>.Empty).ToList())
{
    Console.WriteLine($"\t- {m.Title} ({m.Year})");
}
