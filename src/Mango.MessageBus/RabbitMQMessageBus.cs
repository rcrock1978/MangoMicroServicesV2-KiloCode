using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mango.MessageBus;

/// <summary>
/// RabbitMQ implementation of IMessageBus for event-driven communication
/// </summary>
public class RabbitMQMessageBus : IMessageBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMQMessageBus> _logger;
    private IModel? _channel;

    public RabbitMQMessageBus(ILogger<RabbitMQMessageBus> logger)
    {
        _logger = logger;
        
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
            Port = int.TryParse(Environment.GetEnvironmentVariable("RABBITMQ_PORT"), out var port) ? port : 5672,
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest"
        };

        _connection = factory.CreateConnection();
        _logger.LogInformation("RabbitMQ connection established");
    }

    private IModel GetChannel()
    {
        if (_channel == null || _channel.IsClosed)
        {
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "mango交换", ExchangeType.Direct, durable: true);
        }
        return _channel;
    }

    public Task PublishMessageAsync<T>(T message, string queueName) where T : class
    {
        var channel = GetChannel();
        
        channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.QueueBind(queueName, "mango交换", routingKey: queueName);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        channel.BasicPublish(
            exchange: "mango交换",
            routingKey: queueName,
            basicProperties: properties,
            body: body);

        _logger.LogInformation("Message published to queue {QueueName}", queueName);
        
        return Task.CompletedTask;
    }

    public Task SubscribeAsync<T>(string queueName, Func<T, Task> onMessage) where T : class
    {
        var channel = GetChannel();
        
        channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.QueueBind(queueName, "mango交换", routingKey: queueName);

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new EventingBasicConsumer(channel);
        
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            
            try
            {
                var message = JsonSerializer.Deserialize<T>(json);
                if (message != null)
                {
                    _ = onMessage(message);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message from queue {QueueName}", queueName);
                channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        
        _logger.LogInformation("Subscribed to queue {QueueName}", queueName);
        
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
