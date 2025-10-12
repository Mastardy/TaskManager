using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace API.Models;

public class Card
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    [JsonProperty("_id")]
    public string? Id { get; set; }

    [BsonElement("title")]
    [JsonProperty("title")]
    public required string Title { get; set; }

    [BsonElement("isDone")]
    [JsonProperty("isDone")]
    public bool IsDone { get; set; }

    [BsonElement("description")]
    [JsonProperty("description")]
    public string? Description { get; set; }
}