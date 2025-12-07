using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace API.Models;

public class Card : BaseModel, IMongoDBCollection
{
    public static string CollectionName() => "Cards";

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