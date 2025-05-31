using m165_15_webApi;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

MongoClient client = new MongoClient(builder.Configuration.GetValue<string>("MongoDB:ConnectionString"));
var db = client.GetDatabase(builder.Configuration.GetValue<string>("MongoDB:DatabaseName"));
var moviesCollection = db.GetCollection<Movies>("Movies");

app.MapGet("/", () =>
{
    return Results.Ok((client.ListDatabaseNames().ToList()));
});

// Find

app.MapGet("/movies", () =>
{
    return moviesCollection.Find(_ => true).SortBy(m => m.Year).ToList();
});

app.MapGet("/movies/{id}", (string id) =>
{
    return moviesCollection.Find(m => m.Id == id).FirstOrDefault();
});

// Update

app.MapPut("/movies/{id}", (string id, Movies movie) =>
{
    movie.Id = id;
    moviesCollection.ReplaceOne(m => m.Id == id, movie);
    return Results.Ok(movie);
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
    return Results.NoContent();
});

app.Run();
