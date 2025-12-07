namespace API.Models;

public class MongoDBSettings
{
    public string Host { get; set; } = null!;
    public string UserKey { get; set; } = null!;
    public string PasswordKey { get; set; } = null!;
    public string DatabaseKey { get; set; } = null!;
}