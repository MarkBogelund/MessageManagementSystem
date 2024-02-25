using System;
using System.Threading.Tasks;
using ConsumerApp.DataContext;
using ConsumerApp;
using ConsumerApp.Models;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main()
    {
        // RabbitMQ connection parameters
        string rabbitMQUri = "amqp://guest:guest@rabbitmq:5672";
        string exchangeName = "Message_Broker";
        string routingKey = "Broker_key";
        string queueName = "Message_queue";

        // Running flag
        bool running = true;

        MessageBrokerConsumer messageBroker = new MessageBrokerConsumer(rabbitMQUri, exchangeName, routingKey, queueName);
        
        AppDBContext context = new AppDBContext();
        context.Database.Migrate();

        Database database = new Database(context);
        
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
