using Redis.OM.Modeling;

namespace TaskManager.API.Models;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "Task" })]
public class TaskCache
{
    [RedisIdField, Indexed]
    public string? Id { get; set; }

    [Indexed]
    public string? Title { get; set; }

    [RedisField]
    public string? Description { get; set; }
}