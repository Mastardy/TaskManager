using Microsoft.Extensions.Options;
using MongoDB.Driver;
using API.Utils;
using API.Models;

namespace API.Services.Repositories;

public class MongoDBService
{
    private readonly IMongoCollection<User> m_UserCollection;
    private readonly IMongoCollection<Card> m_CardCollection;

    public MongoDBService(IOptions<MongoDBSettings> settings)
    {
        var MongoSettings = settings.Value;
        var CardsSettings = MongoSettings.Cards;

        var user = EnvHelper.Get(CardsSettings.UserKey);
        var password = EnvHelper.Get(CardsSettings.PasswordKey);
        var database = EnvHelper.Get(CardsSettings.DatabaseKey);
        var collection = EnvHelper.Get(CardsSettings.CollectionKey);

        var mongoURI = $"mongodb://{user}:{password}@{MongoSettings.Host}/admin?authSource={database}";

        var mongoClient = new MongoClient(mongoURI);
        var mongoDatabase = mongoClient.GetDatabase(database);
        m_CardCollection = mongoDatabase.GetCollection<Card>(collection);
        m_UserCollection = mongoDatabase.GetCollection<User>(user);
    }

    public async Task<List<Card>> GetAllCardsAsync() => await m_CardCollection.Find(_ => true).ToListAsync();

    public async Task<Card?> GetCardAsync(string id) =>
        await m_CardCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateCardAsync(Card newCard) => await m_CardCollection.InsertOneAsync(newCard);

    public async Task UpdateCardAsync(Card updatedCard) =>
        await m_CardCollection.ReplaceOneAsync(x => x.Id == updatedCard.Id, updatedCard);

    public async Task DeleteCardAsync(string id) => await m_CardCollection.DeleteOneAsync(x => x.Id == id);
}