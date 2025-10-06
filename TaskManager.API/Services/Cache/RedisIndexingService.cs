using Redis.OM;
using TaskManager.API.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.API.Services;

public class RedisIndexingService : IHostedService
{
    private readonly RedisConnectionProvider m_Provider;

    public RedisIndexingService(RedisConnectionProvider provider) => m_Provider = provider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await m_Provider.Connection.CreateIndexAsync(typeof(TaskCache));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}