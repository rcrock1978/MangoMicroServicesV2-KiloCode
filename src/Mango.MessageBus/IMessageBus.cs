namespace Mango.MessageBus;

/// <summary>
/// Interface for message bus operations using RabbitMQ
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// Publishes a message to a specific queue
    /// </summary>
    /// <typeparam name="T">Message type</typeparam>
    /// <param name="message">Message to publish</param>
    /// <param name="queueName">Queue name</param>
    Task PublishMessageAsync<T>(T message, string queueName) where T : class;

    /// <summary>
    /// Subscribes to a queue for consuming messages
    /// </summary>
    /// <typeparam name="T">Message type</typeparam>
    /// <param name="queueName">Queue name</param>
    /// <param name="onMessage">Callback when message is received</param>
    Task SubscribeAsync<T>(string queueName, Func<T, Task> onMessage) where T : class;
}
