using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace API.Models;

public class User : BaseModel, IMongoDBCollection
{
    public static string CollectionName() => "Users";

    [BsonElement("username")]
    [JsonProperty("username")]
    public required string Username { get; set; }

    [BsonElement("github_id")]
    [JsonProperty("github_id")]
    public int GitHubId { get; set; }

    [BsonElement("avatar_url")]
    [JsonProperty("avatar_url")]
    public required string AvatarUrl { get; set; }

    [BsonElement("email")]
    [JsonProperty("email")]
    public required string Email { get; set; }
}