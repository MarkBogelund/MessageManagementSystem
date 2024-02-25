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

        //SendMessageToQueue(messageBrokerProducer);
        //SendMessageToQueue(messageBrokerProducer);

        // Start consuming messages asynchronously
        await ConsumeMessagesAsync(messageBroker, messageBrokerProducer);

        // Terminate the application
        messageBroker.StopConsuming();
    }

    public static void SendMessageToQueue(MessageBrokerProducer messageBrokerProducer)
    {
        // Create message data
        int _counter = 0;
        int _time = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        // Create message
        Message message = new Message
        {
            Id = 1,
            Counter = _counter,
            Time = _time,
        };

        // Publish message
        messageBrokerProducer.PublishMessage(message);
    }

    static async Task ConsumeMessagesAsync(MessageBrokerConsumer messageBroker, MessageBrokerProducer messageBrokerProducer)
    {
        bool shouldReceiveNextMessage = true;
        Message message = null;

        while (message == null)
        {
            message = await messageBroker.StartConsumingAsync();
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
        Console.WriteLine($"Message Received: Counter = {message.Counter}, UnixTime={messageTime}, Time difference={timeDifference}");

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
            //Database.InsertMessage(message);
            Console.WriteLine("Seconds are even -> Inserting into database");

        }
        else
        {
            SendMessageToQueue(messageBrokerProducer);
            Console.WriteLine("Seconds are odd -> Sending to message queue");
        }

        // Write new line
        Console.WriteLine();
    }
}
