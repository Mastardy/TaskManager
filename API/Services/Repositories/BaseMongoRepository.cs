using API.Models;
using MongoDB.Driver;

namespace API.Services.Repositories;

public abstract class BaseMongoRepository<T> where T : IMongoDBCollection
{
    protected readonly IMongoCollection<T> m_Collection;

    public BaseMongoRepository(MongoDBContext context)
    {
        m_Collection = context.GetCollection<T>();
    }
}