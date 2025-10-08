using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Models;

public class Card
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("title")]
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [BsonElement("isDone")]
    [JsonPropertyName("isDone")]
    public bool IsDone { get; set; }

    [BsonElement("description")]
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}