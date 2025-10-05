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
        return new List<Models.Task>();
        // return await m_TaskCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Models.Task?> GetAsync(string id)
    {
        return new Models.Task
        {
            Title = "Test"
        };
        // return await m_TaskCollection.Find(x => x.Id == id).FirstAsync();
    }

    public async Task CreateAsync(Models.Task newTask)
    {
        // await m_TaskCollection.InsertOneAsync(newTask);
    }

    public async Task UpdateAsync(Models.Task updatedTask)
    {
        // await m_TaskCollection.ReplaceOneAsync(x => x.Id == updatedTask.Id, updatedTask);
    }

    public async Task DeleteAsync(string id)
    {
        // await m_TaskCollection.DeleteOneAsync(x => x.Id == id);
    }
}