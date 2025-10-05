using Redis.OM;
using Redis.OM.Searching;
using TaskManager.API.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.API.Services;

public class RedisService
{
    private readonly RedisCollection<TaskCache> m_TaskCache;

    public RedisService(RedisConnectionProvider provider)
    {
        m_TaskCache = (RedisCollection<TaskCache>)provider.RedisCollection<TaskCache>();
    }

    public async Task<List<TaskCache>> GetAllAsync() => (List<TaskCache>)await m_TaskCache.ToListAsync();
    public async Task<TaskCache?> GetAsync(string id) => await m_TaskCache.FindByIdAsync(id);
    public async Task InsertAsync(TaskCache task) => await m_TaskCache.InsertAsync(task);
    public async Task UpdateAsync(TaskCache task) => await m_TaskCache.UpdateAsync(task);
    public async Task DeleteAsync(TaskCache task) => await m_TaskCache.DeleteAsync(task);
}