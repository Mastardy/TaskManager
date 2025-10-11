using Redis.OM;
using Redis.OM.Searching;
using API.Models;

namespace API.Services;

public class RedisService
{
    private readonly RedisCollection<CardCache> m_CardCache;

    public RedisService(RedisConnectionProvider provider)
    {
        m_CardCache = (RedisCollection<CardCache>)provider.RedisCollection<CardCache>();
    }

    public async Task<CardCache?> GetAsync(string id) => await m_CardCache.FindByIdAsync(id);
    public async Task InsertAsync(CardCache card) => await m_CardCache.InsertAsync(card);
    public async Task InsertAsync(CardCache card, TimeSpan ttl) => await m_CardCache.InsertAsync(card, ttl);
    public async Task DeleteAsync(CardCache card) => await m_CardCache.DeleteAsync(card);
    public async Task DeleteAsync(string? id)
    {
        if (string.IsNullOrEmpty(id)) return;
        var card = await GetAsync(id);
        if (card == null) return;
        await m_CardCache.DeleteAsync(card);
    }
}