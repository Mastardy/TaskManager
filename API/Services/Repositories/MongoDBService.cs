using Microsoft.Extensions.Options;
using MongoDB.Driver;
using API.Utils;
using API.Models;
using API.Models.Authentication;

namespace API.Services.Repositories;

public class MongoDBService
{
    private readonly IMongoCollection<User> m_UserCollection;
    private readonly IMongoCollection<Card> m_CardCollection;

    public MongoDBService(IOptions<MongoDBSettings> settings)
    {
        var MongoSettings = settings.Value;

        var user = EnvHelper.Get(MongoSettings.UserKey);
        var password = EnvHelper.Get(MongoSettings.PasswordKey);
        var database = EnvHelper.Get(MongoSettings.DatabaseKey);

        var mongoURI = $"mongodb://{user}:{password}@{MongoSettings.Host}/admin?authSource={database}";

        var mongoClient = new MongoClient(mongoURI);
        var mongoDatabase = mongoClient.GetDatabase(database);
        m_CardCollection = mongoDatabase.GetCollection<Card>("Cards");
        m_UserCollection = mongoDatabase.GetCollection<User>("Users");
    }

    public async Task<List<Card>> GetAllCardsAsync() => await m_CardCollection.Find(_ => true).ToListAsync();

    public async Task<Card?> GetCardAsync(string id) =>
        await m_CardCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateCardAsync(Card newCard) => await m_CardCollection.InsertOneAsync(newCard);

    public async Task UpdateCardAsync(Card updatedCard) =>
        await m_CardCollection.ReplaceOneAsync(x => x.Id == updatedCard.Id, updatedCard);

    public async Task DeleteCardAsync(string id) => await m_CardCollection.DeleteOneAsync(x => x.Id == id);

    public async Task<User?> GetUserAsync(string id)
    {
        return await m_UserCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserAsync(int id)
    {
        return await m_UserCollection.Find(x => x.GitHubId == id).FirstOrDefaultAsync();
    }

    public async Task CreateUserAsync(User user) => await m_UserCollection.InsertOneAsync(user);
    public async Task UpdateUserAsync(User user) => await m_UserCollection.ReplaceOneAsync(x => x.Id == user.Id, user);

    public async Task CreateOrUpdateUserAsync(GitHubUserDto userDto)
    {
        var user = new User()
        {
            GitHubId = userDto.Id,
            Username = userDto.Login,
            AvatarUrl = userDto.AvatarUrl,
            Email = userDto.Email
        };

        var dbUser = await GetUserAsync(userDto.Id);

        if (dbUser == null)
        {
            await CreateUserAsync(user);
        }
        else
        {
            user.Id = dbUser.Id;
            await UpdateUserAsync(user);
        }
    }
}