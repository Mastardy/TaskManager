using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using API.Utils;
using API.Models;
using Newtonsoft.Json;

namespace API.Services.Messaging;

public class RabbitMQService : IAsyncDisposable
{
    private readonly ConnectionFactory m_Factory;

    private IConnection? m_Connection;
    private IChannel? m_Channel;

    private readonly SemaphoreSlim m_ChannelSemaphore = new(1, 1);
    private readonly SemaphoreSlim m_Declaring = new(1, 1);

    private readonly HashSet<string> m_DeclaredQueues = new();

    public RabbitMQService(IOptions<RabbitMQSettings> settings)
    {
        m_Factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = EnvHelper.Get(settings.Value.UserKey),
            Password = EnvHelper.Get(settings.Value.PasswordKey)
        };
    }

    private async Task<IChannel> GetChannelAsync()
    {
        if (m_Channel != null) return m_Channel;

        await m_ChannelSemaphore.WaitAsync();
        try
        {
            if (m_Channel != null) return m_Channel;

            m_Connection = await m_Factory.CreateConnectionAsync();
            m_Channel = await m_Connection.CreateChannelAsync();
            return m_Channel;
        }
        finally
        {
            m_ChannelSemaphore.Release();
        }
    }

    private async Task DeclareQueueAsync(string queueName, IChannel? channel = null)
    {
        if (channel == null) channel = await GetChannelAsync();

        await m_Declaring.WaitAsync();
        try
        {
            if (m_DeclaredQueues.Contains(queueName)) return;

            await channel.QueueDeclareAsync(queueName, true, false, false);
            m_DeclaredQueues.Add(queueName);
        }
        finally
        {
            m_Declaring.Release();
        }
    }

    public async Task PublishAsync<T>(string queueName, T message)
    {
        var channel = await GetChannelAsync();

        await DeclareQueueAsync(queueName);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(string.Empty, queueName, body);
    }

    public async Task<T?> ConsumeAsync<T>(string queueName)
    {
        var channel = await GetChannelAsync();

        await DeclareQueueAsync(queueName);

        var result = await channel.BasicGetAsync(queueName, true);
        if (result == null) return default;

        var json = Encoding.UTF8.GetString(result.Body.ToArray());
        return JsonConvert.DeserializeObject<T>(json);
    }

    public async Task StartConsumingAsync<T>(string queueName, Func<T?, Task> messageHandler)
    {
        var channel = await GetChannelAsync();

        await DeclareQueueAsync(queueName);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_ /*sender*/, evtArgs) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(evtArgs.Body.ToArray());
                var message = JsonConvert.DeserializeObject<T>(json);

                await messageHandler(message);
                await channel.BasicAckAsync(evtArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await channel.BasicNackAsync(evtArgs.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(queueName, false, consumer);
    }

    public async ValueTask DisposeAsync()
    {
        if (m_Channel != null) await m_Channel.CloseAsync();
        if (m_Connection != null) await m_Connection.CloseAsync();
    }
}