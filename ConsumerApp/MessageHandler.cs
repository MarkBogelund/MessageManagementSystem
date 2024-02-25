using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsumerApp.Interfaces;
using ConsumerApp.Models;

namespace ConsumerApp
{
    public class MessageHandler : IMessageHandler
    {
        MessageBrokerConsumer messageBroker;
        Database database;
        private IMessageBrokerConsumer messageBrokerMock;
        private IDatabase databaseMock;

        public MessageHandler(MessageBrokerConsumer _messageBroker, Database _database)
        {
            messageBroker = _messageBroker;
            database = _database;
        }

        public MessageHandler(IMessageBrokerConsumer messageBrokerMock, IDatabase databaseMock)
        {
            this.messageBrokerMock = messageBrokerMock;
            this.databaseMock = databaseMock;
        }

        public void HandleMessage(Message message)
        {
            // Calculate time difference
            int timeDifference = GetTimeDifference(message);

            // Display received message
            Console.WriteLine($"Message {message.Id}, Counter: {message.Counter}, Time: {message.Time}, Difference: {timeDifference}");

            // If message is older than 1 minute, discard it
            if (timeDifference > 60)
            {
                Console.WriteLine("Difference > 1 min -> Discarding");
                Console.WriteLine();
                return;
            }

            // If message is not older than 1 minute, handle it
            HandleData(message);
        }

        public int GetTimeDifference(Message message)
        {
            // Calculate time difference
            int actualTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int messageTime = message.Time;
            return actualTime - messageTime;
        }

        public void HandleData(Message message)
        {
            // Get seconds from message time
            int seconds = message.Time % 60;

            // Check if seconds are even or odd
            switch (seconds % 2)
            {
                case 0: // Even
                    Console.WriteLine("Seconds are even -> Inserting into database");
                    database.InsertMessage(message);
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

            // Make sure to separate messages
            Console.WriteLine();
        }
    }
}