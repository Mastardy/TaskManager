using Messaging;

namespace LoggerService;

public class Worker : IHostedService
{
    private readonly ILogger<Worker> m_Logger;
    private RabbitMQService m_RabbitMQService;
    private Dictionary<string, IQueueHandler> m_Queues;

    public Worker(ILogger<Worker> logger, RabbitMQService rabbitMQService)
    {
        m_Logger = logger;
        m_RabbitMQService = rabbitMQService;

        var factory = new QueueHandlerFactory(rabbitMQService);
        m_Queues = new Dictionary<string, IQueueHandler>
        {
            { ".cardcreated", factory.Create<string?>(OnCardCreated) },
            { "card.updated", factory.Create<string?>(OnCardUpdated) },
            { "card.deleted", factory.Create<string?>(OnCardDeleted) }
        };
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var queue in m_Queues)
        {
            await queue.Value.StartConsumingAsync(queue.Key);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var queue in m_Queues)
        {
            await m_RabbitMQService.StopConsumingAsync(queue.Key);
        }
    }

    private Task OnCardCreated(string? cardId)
    {
        m_Logger.LogInformation($"Card {cardId ?? "Null"} created.");
        return Task.CompletedTask;
    }

    private Task OnCardUpdated(string? cardId)
    {
        m_Logger.LogInformation($"Card {cardId ?? "Null"} updated.");
        return Task.CompletedTask;
    }

    private Task OnCardDeleted(string? cardId)
    {
        m_Logger.LogInformation($"Card {cardId ?? "Null"} deleted.");
        return Task.CompletedTask;
    }
}