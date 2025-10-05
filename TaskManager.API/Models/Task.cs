using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Redis.OM.Modeling;

namespace TaskManager.API.Models;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "Task" })]
public class Task
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [RedisIdField]
    [Indexed]
    public string? Id { get; set; }

    [BsonElement("title")]
    [JsonPropertyName("title")]
    [Indexed]
    public required string Title { get; set; }

    [BsonElement("isDone")]
    [JsonPropertyName("isDone")]
    [Indexed]
    public bool IsDone { get; set; }

    [BsonElement("description")]
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}