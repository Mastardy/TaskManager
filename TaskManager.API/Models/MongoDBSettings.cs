namespace TaskManager.API.Models;

public class MongoDBSettings
{
    public string Host { get; set; } = null!;
    public MongoDBTasks Tasks { get; set; } = null!;
}

[Serializable]
public class MongoDBTasks
{
    public string UserKey { get; set; } = null!;
    public string PasswordKey { get; set; } = null!;
    public string DatabaseKey { get; set; } = null!;
    public string CollectionKey { get; set; } = null!;
}