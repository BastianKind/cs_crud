using m165_15_webApi;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const string connectionString = "mongodb://localhost:27017";
MongoClient client = new MongoClient(connectionString);
var db = client.GetDatabase("M165");
var moviesCollection = db.GetCollection<Movies>("Movies");

app.MapGet("/", () =>
{
    var html = "<h1>Databases</h1><br><ul>";
    foreach (var dbName in client.ListDatabaseNames().ToList())
    {
        html += $"<li>{dbName}</li>";
    }
    html += "</ul>";
    return Results.Content(html, "text/html");
});

// Find

app.MapGet("/movies", () =>
{
    return Results.Content(moviesCollection.Find(m => true)
        .SortBy(m => m.Year)
        .ToList()
        .ToJson(), "application/json");
});

app.MapGet("/movies/{id}", (string id) =>
{
    return Results.Content(moviesCollection
        .Find(m => m.Id == id)
        .FirstOrDefault()
        .ToJson(), "application/json");
});

// Update

app.MapPut("/movies/{id}", (string id, Movies movie) =>
{
    movie.Id = id;
    moviesCollection
        .ReplaceOne(m => m.Id == id, movie);
    
    return Results.Content(moviesCollection
        .Find(m => m.Id == id)
        .FirstOrDefault()
        .ToJson(), "application/json");
});

// Create
app.MapPost("/movies", (Movies movie) =>
{
    moviesCollection.InsertOne(movie);
    return Results.Created($"/movies/{movie.Id}", movie);
});
// Kill
app.MapDelete("/movies/{id}", (string id) =>
{
    moviesCollection.DeleteOne(m => m.Id == id);
    return Results.StatusCode(statusCode: 204);
});

app.Run();
