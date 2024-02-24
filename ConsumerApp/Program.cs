using System;
using System.Threading.Tasks;
using ConsumerApp.DataContext;
using ConsumerApp.Models;
using ConsumerApp;

class Program
{
    static async Task Main()
    {
        string rabbitMQUri = "amqp://guest:guest@localhost:5672";
        string exchangeName = "Message_Broker";
        string routingKey = "Broker_key";
        string queueName = "Message_queue";

        var rabbitMQConsumer = new RabbitMQConsumer(rabbitMQUri, exchangeName, routingKey, queueName);

        // Start consuming messages asynchronously
        await ConsumeMessagesAsync(rabbitMQConsumer);
    }

    static async Task ConsumeMessagesAsync(RabbitMQConsumer rabbitMQConsumer)
    {
        while (true)
        {
            // Retrieve message asynchronously
            MessageData message = await rabbitMQConsumer.StartConsumingAsync();

            // Calculate time difference
            int actualTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int timeDifference = actualTime - message.unixTime;

            // Display received message
            Console.WriteLine($"Message Received: Counter = {message.counter}, UnixTime={message.unixTime}, Time difference={timeDifference}");
        }
    }
}
