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
            Message message = await rabbitMQConsumer.StartConsumingAsync();

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
                continue;
            }

            int seconds = messageTime % 60;

            if (seconds % 2 == 0)
            {
                // Seconds are even
                Database.InsertMessage(message);
                Console.WriteLine("Seconds are even -> Inserting into database");
            }
            else
            {
                // Seconds are odd
                message.Counter++;
                //SendToMessageQueue(message);
                Console.WriteLine("Seconds are odd -> Sending to message queue");
            }

        }
    }
}
