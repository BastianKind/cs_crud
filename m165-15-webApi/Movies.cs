using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace m165_15_webApi;

public class Movies
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }

    public string? Title { get; set; }

    public int? Year { get; set; }

    public string? Summary { get; set; }

    public List<string>? Actors { get; set; }
}