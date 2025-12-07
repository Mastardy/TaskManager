using API.Models;

namespace API.Services.Repositories;

public class CardRepository : BaseMongoRepository<Card>
{
    public CardRepository(MongoDBContext context) : base(context)
    {
        Console.WriteLine("Getting Collection Name: " + m_Collection.CollectionNamespace.CollectionName);
    }
}