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

        Console.WriteLine("Consuming messages...");

        // Start consuming messages asynchronously
        await ConsumeMessagesAsync(messageBroker);

        // Terminate the application
        messageBroker.StopConsuming();
    }

    static async Task ConsumeMessagesAsync(MessageBrokerConsumer messageBroker)
    {
        while (true)
        {
            // If a new message is received, handle it
            var message = await messageBroker.StartConsumingAsync();

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
                continue;
            }

            // If message is not older than 1 minute, handle it
            HandleData(message, messageBroker);
        }
    }

    static void HandleData(Message message, MessageBrokerConsumer messageBroker)
    {
        // Get seconds from message time
        int seconds = message.Time % 60;

        // Check if seconds are even or odd
        switch (seconds % 2)
        {
            case 0: // Even
                Console.WriteLine("Seconds are even -> Inserting into database");
                //Database.InsertMessage(message);
                break;

            default: // Odd
                Console.WriteLine("Seconds are odd -> Sending to message queue");

                // Wait for 1 second
                Task.Delay(1000).Wait();

                // Increment counter and update time
                message.Counter++;
                message.Time = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                // Publish message
                messageBroker.SendMessageToQueue(message);
                break;
        }

        // Write new line
        Console.WriteLine();
    }
}
