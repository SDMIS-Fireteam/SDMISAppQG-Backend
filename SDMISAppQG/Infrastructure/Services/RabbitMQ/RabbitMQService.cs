using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SDMISAppQG.Infrastructure.Services.RabbitMQ;

public class RabbitMQService : IRabbitMQService, IDisposable
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQService> _logger;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public RabbitMQService(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    private async Task EnsureConnectionAsync()
    {
        if (_connection != null && _connection.IsOpen && _channel != null && _channel.IsOpen)
            return;

        await _connectionLock.WaitAsync();
        try
        {
            if (_connection != null && _connection.IsOpen && _channel != null && _channel.IsOpen)
                return;

            _logger.LogInformation("Connecting to RabbitMQ at {Host}:{Port}", _settings.HostName, _settings.Port);

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            // Declare exchange
            await _channel.ExchangeDeclareAsync(
                exchange: _settings.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false
            );

            // Declare queues
            await _channel.QueueDeclareAsync(
                queue: _settings.QueueNames.ToJava,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await _channel.QueueDeclareAsync(
                queue: _settings.QueueNames.FromJava,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // Bind queues to exchange
            await _channel.QueueBindAsync(
                queue: _settings.QueueNames.ToJava,
                exchange: _settings.ExchangeName,
                routingKey: _settings.QueueNames.ToJava
            );

            await _channel.QueueBindAsync(
                queue: _settings.QueueNames.FromJava,
                exchange: _settings.ExchangeName,
                routingKey: _settings.QueueNames.FromJava
            );

            _logger.LogInformation("Connected to RabbitMQ successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ");
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async void PublishMessage<T>(T message, string queueName)
    {
        try
        {
            await EnsureConnectionAsync();

            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _channel!.BasicPublishAsync(
                exchange: _settings.ExchangeName,
                routingKey: queueName,
                mandatory: false,
                basicProperties: properties,
                body: body
            );

            _logger.LogInformation("Published message to queue {Queue}: {Message}", queueName, jsonMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to queue {Queue}", queueName);
            throw;
        }
    }

    public void PublishToJava<T>(T message)
    {
        PublishMessage(message, _settings.QueueNames.ToJava);
    }

    public async void Subscribe(string queueName, Action<string> onMessageReceived)
    {
        try
        {
            await EnsureConnectionAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel!);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    
                    _logger.LogInformation("Received message from queue {Queue}: {Message}", queueName, message);
                    
                    onMessageReceived(message);
                    
                    await _channel!.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from queue {Queue}", queueName);
                    await _channel!.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer
            );

            _logger.LogInformation("Subscribed to queue {Queue}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to queue {Queue}", queueName);
            throw;
        }
    }

    public void SubscribeFromJava(Action<string> onMessageReceived)
    {
        Subscribe(_settings.QueueNames.FromJava, onMessageReceived);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        _connectionLock.Dispose();
        _logger.LogInformation("RabbitMQ connection disposed");
    }
}
