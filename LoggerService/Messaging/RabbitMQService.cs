using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Utils;

namespace Messaging;

public class RabbitMQService : IAsyncDisposable
{
    private readonly ConnectionFactory m_Factory;

    private IConnection? m_Connection;
    private IChannel? m_Channel;

    private readonly SemaphoreSlim m_ChannelSemaphore = new(1, 1);

    private Dictionary<string, string> m_ConsumerTags = new();

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

    public async Task StartConsumingAsync<T>(string queueName, Func<T?, Task> messageHandler)
    {
        var channel = await GetChannelAsync();

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_ /*sender*/, evtArgs) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(evtArgs.Body.ToArray());
                var message = JsonSerializer.Deserialize<T>(json);

                await messageHandler(message);
                await channel.BasicAckAsync(evtArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await channel.BasicNackAsync(evtArgs.DeliveryTag, multiple: false, requeue: true);
            }
        };

        m_ConsumerTags[queueName] = await channel.BasicConsumeAsync(queueName, false, consumer);
    }

    public async Task StopConsumingAsync(string queueName)
    {
        if (!m_ConsumerTags.ContainsKey(queueName)) return;

        var channel = await GetChannelAsync();
        await channel.BasicCancelAsync(m_ConsumerTags[queueName]);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var queueName in m_ConsumerTags.Keys)
        {
            await StopConsumingAsync(queueName);
        }

        if (m_Channel != null) await m_Channel.CloseAsync();
        if (m_Connection != null) await m_Connection.CloseAsync();
    }
}