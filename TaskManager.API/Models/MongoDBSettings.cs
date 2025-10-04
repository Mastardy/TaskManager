namespace TaskManager.API.Models;

public class MongoDBSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string TasksCollectionName { get; set; } = null!;
}