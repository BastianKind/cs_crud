using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Movies
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int Year { get; set; }

    public string Summary { get; set; } = string.Empty;

    public List<string> Actors { get; set; } = [];
}