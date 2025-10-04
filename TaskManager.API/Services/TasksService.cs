using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TaskManager.API.Models;

namespace TaskManager.API.Services;

public class TasksService
{
    private readonly IMongoCollection<Models.Task> _taskCollection;

    public TasksService(IOptions<MongoDBSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _taskCollection = mongoDatabase.GetCollection<Models.Task>(settings.Value.TasksCollectionName);
    }

    public async Task<List<Models.Task>> GetTasksAsync()
    {
        return await _taskCollection.Find(_ => true).ToListAsync();
    }
}