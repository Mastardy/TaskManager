using Redis.OM.Modeling;

namespace API.Models;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "Card" })]
public class CardCache
{
    [RedisIdField, Indexed]
    public string? Id { get; set; }

    [Indexed]
    public string Title { get; set; } = null!;

    [RedisField]
    public string? Description { get; set; }

    [RedisField]
    public bool IsDone { get; set; }

    public static implicit operator Card(CardCache cache)
    {
        return new Card()
        {
            Id = cache.Id,
            Title = cache.Title,
            Description = cache.Description,
            IsDone = cache.IsDone,
        };
    }

    public static implicit operator CardCache(Card data)
    {
        return new CardCache()
        {
            Id = data.Id,
            Title = data.Title,
            Description = data.Description,
            IsDone = data.IsDone,
        };
    }
}