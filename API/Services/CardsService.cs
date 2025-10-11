using API.Services.Messaging;
using API.Services.Repositories;
using API.Models;

namespace API.Services;

public class CardsService
{
    private readonly MongoDBService m_MongoDBService;
    private readonly RedisService m_RedisService;
    private readonly RabbitMQService m_RabbitMQService;

    public static class EventType
    {
        public static readonly string CARD_CREATED = "card.created";
        public static readonly string CARD_UPDATED = "card.updated";
        public static readonly string CARD_DELETED = "card.deleted";
    }

    public CardsService(MongoDBService mongoDBService, RedisService redisService, RabbitMQService rabbitMQService)
    {
        m_MongoDBService = mongoDBService;
        m_RedisService = redisService;
        m_RabbitMQService = rabbitMQService;
    }

    public async Task<List<Card>> GetAllAsync()
    {
        return await m_MongoDBService.GetAllAsync();
    }

    public async Task<Card?> GetAsync(string id)
    {
        var cache = await m_RedisService.GetAsync(id);
        if (cache != null) return cache;

        var result = await m_MongoDBService.GetAsync(id);
        if (result == null) return null;

        await m_RedisService.InsertAsync(result, TimeSpan.FromMinutes(10));

        return result;
    }

    public async Task CreateAsync(Card newCard)
    {
        await m_RedisService.InsertAsync(newCard, TimeSpan.FromMinutes(10));
        await m_MongoDBService.CreateAsync(newCard);
        await m_RabbitMQService.PublishAsync(EventType.CARD_CREATED, newCard.Id);
    }

    public async Task UpdateAsync(Card updatedCard)
    {
        await m_RedisService.DeleteAsync(updatedCard.Id);
        await m_MongoDBService.UpdateAsync(updatedCard);
        await m_RabbitMQService.PublishAsync(EventType.CARD_UPDATED, updatedCard.Id);
    }

    public async Task DeleteAsync(string id)
    {
        await m_RedisService.DeleteAsync(id);
        await m_MongoDBService.DeleteAsync(id);
        await m_RabbitMQService.PublishAsync(EventType.CARD_DELETED, id);
    }
}