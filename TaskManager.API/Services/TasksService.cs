using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TaskManager.API.Services.Repositories;
using TaskManager.API.Utils;

namespace TaskManager.API.Services;

public class TasksService
{
    private readonly MongoDBService m_MongoDBService;
    private readonly RedisService m_RedisService;

    public TasksService(MongoDBService mongoDBService, RedisService redisService)
    {
        m_MongoDBService = mongoDBService;
        m_RedisService = redisService;
    }

    public async Task<List<Models.Task>> GetAllAsync()
    {
        return await m_MongoDBService.GetAllAsync();
    }

    public async Task<Models.Task?> GetAsync(string id)
    {
        var cache = await m_RedisService.GetAsync(id);
        if (cache != null) return cache;

        var result = await m_MongoDBService.GetAsync(id);
        if (result == null) return null;

        // TODO : This should not delay the retrieval, perhaps call RabbitMQ or Fire & Forget?
        await m_RedisService.InsertAsync(result, TimeSpan.FromMinutes(1));
        return result;
    }

    public async Task CreateAsync(Models.Task newTask)
    {
        await m_RedisService.InsertAsync(newTask, TimeSpan.FromMinutes(1));
        await m_MongoDBService.CreateAsync(newTask);
    }

    public async Task UpdateAsync(Models.Task updatedTask)
    {
        await m_RedisService.DeleteAsync(updatedTask.Id);
        await m_MongoDBService.UpdateAsync(updatedTask);
    }

    public async Task DeleteAsync(string id)
    {
        await m_RedisService.DeleteAsync(id);
        await m_MongoDBService.DeleteAsync(id);
    }
}