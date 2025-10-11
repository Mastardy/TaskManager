using Messaging;

namespace LoggerService;

public interface IQueueHandler
{
    Task StartConsumingAsync(string queueName);
}

public class QueueHandler<T> : IQueueHandler
{
    private readonly RabbitMQService m_RabbitMQService;
    private readonly Func<T?, Task> m_Handler;

    public QueueHandler(RabbitMQService service, Func<T?, Task> handler)
    {
        m_RabbitMQService = service;
        m_Handler = handler;
    }

    public async Task StartConsumingAsync(string queueName)
    {
        await m_RabbitMQService.StartConsumingAsync(queueName, m_Handler);
    }
}

public class QueueHandlerFactory
{
    private readonly RabbitMQService m_RabbitMQService;

    public QueueHandlerFactory(RabbitMQService rabbitMQService)
    {
        m_RabbitMQService = rabbitMQService;
    }

    public IQueueHandler Create<T>(Func<T, Task> handler)
    {
        return new QueueHandler<T>(m_RabbitMQService, handler);
    }
}