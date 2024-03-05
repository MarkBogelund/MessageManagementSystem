using System;
using System.Threading.Tasks;
using ConsumerApp.DataContext;
using ConsumerApp;
using ConsumerApp.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

class Program
{
    static async Task Main()
    {
        // RabbitMQ connection parameters
        string rabbitMQUri = Environment.GetEnvironmentVariable("RABBITMQ_URI")!; // For Docker
        string localRabbitMQUri = Environment.GetEnvironmentVariable("LOCAL_RABBITMQ_URI")!; // For development
        
        string exchangeName = "Message_Broker";
        string routingKey = "Broker_key";
        string queueName = "Message_queue";

        // Running flag
        bool running = true;

        // Create IConnection
        var factory = new ConnectionFactory
        {
            Uri = new Uri(Environment.GetEnvironmentVariable("ENVIRONMENT") == "development" ? localRabbitMQUri : rabbitMQUri),
            ClientProvidedName = "Consumer"
        };

        // Create an instance of MessageBrokerConsumer
        var messageBroker = new MessageBrokerConsumer(factory, exchangeName, routingKey, queueName);

        AppDBContext context = new AppDBContext();
        context.Database.Migrate();

        Database database = new Database(context);

        MessageHandler messageHandler = new MessageHandler(messageBroker, database);

        Console.WriteLine("Consuming messages...");

        while (running)
        {
            // If a new message is received, handle it
            Message message = await messageBroker.RecieveMessageAsync();

            int timeDifference = messageHandler.GetTimeDifference(message);

            // Display received message
            Console.WriteLine($"Message {message.Id}, Counter: {message.Counter}, Time: {message.Time}, Difference: {timeDifference}");

            if (timeDifference > 60)
                Console.WriteLine("Difference > 1 min -> Discarding\n");
            else
                messageHandler.HandleMessage(message);
        }

        // Terminate the application
        messageBroker.StopConsuming();
    }
}