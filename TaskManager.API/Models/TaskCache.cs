using Redis.OM.Modeling;

namespace TaskManager.API.Models;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "Task" })]
public class TaskCache
{
    [RedisIdField, Indexed]
    public string? Id { get; set; }

    [Indexed]
    public string Title { get; set; } = null!;

    [RedisField]
    public string? Description { get; set; }

    [RedisField]
    public bool IsDone { get; set; }

    public static implicit operator Task(TaskCache cache)
    {
        return new Task()
        {
            Id = cache.Id,
            Title = cache.Title,
            Description = cache.Description,
            IsDone = cache.IsDone,
        };
    }

    public static implicit operator TaskCache(Task data)
    {
        return new TaskCache()
        {
            Id = data.Id,
            Title = data.Title,
            Description = data.Description,
            IsDone = data.IsDone,
        };
    }
}