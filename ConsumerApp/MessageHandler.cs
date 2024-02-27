using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsumerApp.Interfaces;
using ConsumerApp.Models;

namespace ConsumerApp
{
    public class MessageHandler
    {
        private readonly IMessageBrokerConsumer messageBroker;
        private readonly IDatabase database;

        public MessageHandler(IMessageBrokerConsumer messageBroker, IDatabase database)
        {
            this.messageBroker = messageBroker ?? throw new ArgumentNullException(nameof(messageBroker));
            this.database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public void HandleMessage(Message message)  
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

        public int GetTimeDifference(Message message)
        {
            // Calculate time difference
            int actualTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int messageTime = message.Time;
            return actualTime - messageTime;
        }
    }
}