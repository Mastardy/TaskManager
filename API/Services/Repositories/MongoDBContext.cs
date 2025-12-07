using API.Models;
using API.Utils;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace API.Services.Repositories;

public class MongoDBContext
{
    private readonly IMongoDatabase m_Database;

    public MongoDBContext(IOptions<MongoDBSettings> settings)
    {
        var MongoSettings = settings.Value;

        var user = EnvHelper.Get(MongoSettings.UserKey);
        var password = EnvHelper.Get(MongoSettings.PasswordKey);
        var database = EnvHelper.Get(MongoSettings.DatabaseKey);

        var mongoURI = $"mongodb://{user}:{password}@{MongoSettings.Host}/admin?authSource={database}";
        IMongoClient mClient = new MongoClient(mongoURI);
        m_Database = mClient.GetDatabase(database);
    }

    public IMongoCollection<T> GetCollection<T>() where T : IMongoDBCollection
    {
        return m_Database.GetCollection<T>(T.CollectionName());
    }
}