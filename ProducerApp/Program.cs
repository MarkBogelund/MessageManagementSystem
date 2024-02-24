using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ProducerApp.Test")]
namespace ProducerApp
{
    internal class Program
    {
        public static void Main()
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
                int _time = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                // Create message
                Message message = new Message
                {
                    Counter = _counter,
                    Time = _time
                };

                // Publish message
                messageBroker.PublishMessage(message);

                // Log message
                Console.WriteLine($"Sent message with counter: '{message.Counter}' " +
                                    $"and current time (Unix): {message.Time}\n");

                // Delay for 1 second
                Thread.Sleep(1000);
            }

            messageBroker.CloseConnection();
        }

        // Test method
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}