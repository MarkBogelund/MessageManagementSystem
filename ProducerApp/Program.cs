using RabbitMQ.Client;
using System.Runtime.CompilerServices;

namespace ProducerApp
{
    internal class Program
    {
        public static void Main()
        {
            // Running flag
            bool running = true;

            // Connection properties for RabbitMQ
            string rabbitMQUri = Environment.GetEnvironmentVariable("RABBITMQ_URI")!; // For Docker
            string localRabbitMQUri = Environment.GetEnvironmentVariable("LOCAL_RABBITMQ_URI")!; // For development
            string exchangeName = "Message_Broker";
            string routingKey = "Broker_key";
            string queueName = "Message_queue";

            IConnectionFactory factory = new ConnectionFactory
            {
                Uri = new Uri(Environment.GetEnvironmentVariable("ENVIRONMENT") == "development" ? localRabbitMQUri : rabbitMQUri)
            };

            var messageBroker = new MessageBrokerProducer(factory, exchangeName, routingKey, queueName);

            int id_counter = 0;

            Console.WriteLine("Producer started\n");

            // Send a message every second
            while (running)
            {
                // Get unique id
                id_counter++;

                // Create message
                Message message = messageBroker.CreateMessage(id_counter);

                // Publish message
                messageBroker.PublishMessage(message);

                // Log message
                Console.WriteLine($"Message {message.Id}, Counter: {message.Counter}, Time: {message.Time}\n");

                // Delay for 1 second
                Thread.Sleep(1000);
            }

            messageBroker.CloseConnection();
        }

       
    }

}