using ProducerApp;
class Program
{
    static void Main()
    {
        // Running flag
        bool running = true;

        // Connection properties for RabbitMQ
        string rabbitMQUri = "amqp://guest:guest@localhost:5672";
        string exchangeName = "Message_Broker";
        string routingKey = "Broker_key";
        string queueName = "Message_queue";

        var messageBroker = new MessageBrokerProducer(rabbitMQUri, exchangeName, routingKey, queueName);

        // Send a message every second
        while (running)
        {
            // Create message data
            int _counter = 0;
            int _unixTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // Create message
            Message message = new Message
            {
                counter = _counter,
                unixTime = _unixTime
            };

            // Publish message
            messageBroker.PublishMessage(message);
            
            // Log message
            Console.WriteLine($"Sent message with counter: '{message.counter}' " +
                                $"and current time (Unix): {message.unixTime}\n");
            
            // Delay for 1 second
            Thread.Sleep(1000);
        }

        messageBroker.CloseConnection();
    }
}