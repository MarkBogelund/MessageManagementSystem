﻿using System.Runtime.CompilerServices;

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

            int id_counter = 0;

            // Send a message every second
            while (running)
            {
                // Get unique message id
                id_counter++;
                
                // Create message data
                int _counter = 0;
                int _time = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                int id = id_counter;

                // Create message
                Message message = new Message
                {
                    Id = id,
                    Counter = _counter,
                    Time = _time
                };

                // Publish message
                messageBroker.PublishMessage(message);

                // Log message
                Console.WriteLine($"Message {message.Id}, Counter: '{message.Counter}, Time: {message.Time}\n");

                // Delay for 1 second
                Thread.Sleep(1000);
            }

            messageBroker.CloseConnection();
        }
    }
}