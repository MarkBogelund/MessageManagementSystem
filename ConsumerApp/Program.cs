using System;
using System.Threading.Tasks;
using ConsumerApp.DataContext;
using ConsumerApp.Models;
using ConsumerApp;

class Program
{
    static async Task Main()
    {
        // RabbitMQ connection parameters
        string rabbitMQUri = "amqp://guest:guest@localhost:5672";
        string exchangeName = "Message_Broker";
        string routingKey = "Broker_key";
        string queueName = "Message_queue";

        MessageBrokerConsumer messageBroker = new MessageBrokerConsumer(rabbitMQUri, exchangeName, routingKey, queueName);
        var messageBrokerProducer = new MessageBrokerProducer(rabbitMQUri, exchangeName, routingKey, queueName);

        Console.WriteLine("Press [enter] to exit");

        // Start consuming messages asynchronously
        await ConsumeMessagesAsync(messageBroker, messageBrokerProducer);

        // Terminate the application
        messageBroker.StopConsuming();
    }

    public static void SendMessageToQueue(MessageBrokerProducer messageBrokerProducer, Message message)
    {
        // Wait for 1 second
        Task.Delay(1000).Wait();

        // Increment counter and update time
        message.Counter++;
        message.Time = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        // Publish message
        messageBrokerProducer.PublishMessage(message);
    }

    static async Task ConsumeMessagesAsync(MessageBrokerConsumer messageBroker, MessageBrokerProducer messageBrokerProducer)
    {
        while (true)
        {
            // If a new message is received, handle it
            var message = await messageBroker.StartConsumingAsync();
            HandleData(message, messageBrokerProducer);
        }
    }

    static void HandleData(Message message, MessageBrokerProducer messageBrokerProducer)
    {
        // Calculate time difference
        int actualTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        int messageTime = message.Time;
        int timeDifference = actualTime - messageTime;

        // Display received message
        Console.WriteLine($"Message {message.Id}, Counter: '{message.Counter}, Time: {message.Time}, Difference: {timeDifference}");

        // If message is older than 1 minute, discard it
        if (timeDifference > 60)
        {
            Console.WriteLine("Difference > 1 min -> Discarding");
            Console.WriteLine();
            return;
        }

        // Get seconds from message time
        int seconds = messageTime % 60;

        // Check if seconds are even or odd
        if (seconds % 2 == 0)
        {
            // Seconds are even
            Console.WriteLine("Seconds are even -> Inserting into database");
            //Database.InsertMessage(message);
        }
        else
        {
            Console.WriteLine("Seconds are odd -> Sending to message queue");
            SendMessageToQueue(messageBrokerProducer, message);
        }

        // Write new line
        Console.WriteLine();
    }
}
