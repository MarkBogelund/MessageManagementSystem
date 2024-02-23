class Program
{
    static void Main()
    {
        string rabbitMQUri = "amqp://guest:guest@localhost:5672";
        string exchangeName = "Message_Broker";
        string routingKey = "Broker_key";
        string queueName = "Message_queue";
        int messageCount = 10;

        var rabbitMQPublisher = new RabbitMQPublisher(rabbitMQUri, exchangeName, routingKey, queueName, messageCount);
        rabbitMQPublisher.PublishMessageWithTime();
        rabbitMQPublisher.CloseConnection();
    }
}