using System;
using System.Threading.Tasks;
using ConsumerApp.DataContext;
using ConsumerApp;
using ConsumerApp.Models;

class Program
{
    static async Task Main()
    {
        // RabbitMQ connection parameters
        string rabbitMQUri = "amqp://guest:guest@localhost:5672";
        string exchangeName = "Message_Broker";
        string routingKey = "Broker_key";
        string queueName = "Message_queue";

        bool running = true;

        MessageBrokerConsumer messageBroker = new MessageBrokerConsumer(rabbitMQUri, exchangeName, routingKey, queueName);
        Database database = new Database();
        MessageHandler messageHandler = new MessageHandler(messageBroker, database);

        Console.WriteLine("Consuming messages...");

        while (running)
        {
            // If a new message is received, handle it
            Message message = await messageBroker.StartConsumingAsync();
            messageHandler.HandleMessage(message);
        }

        // Terminate the application
        messageBroker.StopConsuming();
    }
}
