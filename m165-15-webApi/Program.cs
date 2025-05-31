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
    return Results.Ok(moviesCollection.Find(_ => true).SortBy(m => m.Year).ToList());
});

app.MapGet("/movies/{_id}", (string _id) =>
{
    return Results.Ok(moviesCollection.Find(m => m._id == _id).FirstOrDefault());
});

// Update

app.MapPut("/movies/{objectId}", (string objectId, Movies changes) =>
{
    /*
        movie._id = _id;
        moviesCollection.ReplaceOne(m => m._id == _id, movie);
        return Results.Ok(movie);
    */
    var updates = changes.GetType()
        .GetProperties()
        .Where(prop => prop.GetValue(changes) is {} value)
        .Select(prop => Builders<Movies>.Update.Set(prop.Name, prop.GetValue(changes)))
        .ToList();

    var result = moviesCollection.UpdateOne(
        Builders<Movies>.Filter.Eq("_id", objectId),
        Builders<Movies>.Update.Combine(updates));

    return Results.Ok(result);
});

// Create
app.MapPost("/movies", (Movies movie) =>
{
    moviesCollection.InsertOne(movie);
    return Results.Created($"/movies/{movie._id}", movie);
});
// Kill
app.MapDelete("/movies/{_id}", (string _id) =>
{
    moviesCollection.DeleteOne(m => m._id == _id);
    return Results.NoContent();
});

app.Run();
