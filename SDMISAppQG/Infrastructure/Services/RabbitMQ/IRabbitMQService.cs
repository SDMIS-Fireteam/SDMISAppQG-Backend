namespace SDMISAppQG.Infrastructure.Services.RabbitMQ;

public interface IRabbitMQService
{
    /// <summary>
    /// Publish a message to a specific queue
    /// </summary>
    void PublishMessage<T>(T message, string queueName);

    /// <summary>
    /// Publish a message to the Java service queue
    /// </summary>
    void PublishToJava<T>(T message);

    /// <summary>
    /// Subscribe to messages from a specific queue
    /// </summary>
    void Subscribe(string queueName, Action<string> onMessageReceived);

    /// <summary>
    /// Subscribe to messages from the Java service queue
    /// </summary>
    void SubscribeFromJava(Action<string> onMessageReceived);
}
