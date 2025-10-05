using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TaskManager.API.Utils;

namespace TaskManager.API.Services.Repositories;

public class MongoDBService
{
    private readonly IMongoCollection<Models.Task> m_TaskCollection;

    public MongoDBService(IOptions<Models.MongoDBSettings> settings)
    {
        var MongoSettings = settings.Value;
        var TasksSettings = MongoSettings.Tasks;

        var user = EnvHelper.Get(TasksSettings.UserKey);
        var password = EnvHelper.Get(TasksSettings.PasswordKey);
        var database = EnvHelper.Get(TasksSettings.DatabaseKey);
        var collection = EnvHelper.Get(TasksSettings.CollectionKey);

        var mongoURI = $"mongodb://{user}:{password}@{MongoSettings.Host}/admin?authSource={database}";

        var mongoClient = new MongoClient(mongoURI);
        var mongoDatabase = mongoClient.GetDatabase(database);
        m_TaskCollection = mongoDatabase.GetCollection<Models.Task>(collection);
    }

    public async Task<List<Models.Task>> GetAsync() => await m_TaskCollection.Find(_ => true).ToListAsync();

    public async Task<Models.Task?> GetAsync(string id) => await m_TaskCollection.Find(x => x.Id == id).FirstAsync();

    public async Task CreateAsync(Models.Task newTask) => await m_TaskCollection.InsertOneAsync(newTask);

    public async Task UpdateAsync(Models.Task updatedTask) =>
        await m_TaskCollection.ReplaceOneAsync(x => x.Id == updatedTask.Id, updatedTask);

    public async Task DeleteAsync(string id) => await m_TaskCollection.DeleteOneAsync(x => x.Id == id);
}