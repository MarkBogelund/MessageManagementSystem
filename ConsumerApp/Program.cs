﻿using System;
using Npgsql.Replication.PgOutput.Messages;
using ConsumerApp.DataContext;
using ConsumerApp.Models;
using ConsumerApp;

class Program
{
    static void Main()
    {
        string rabbitMQUri = "amqp://guest:guest@localhost:5672";
        string exchangeName = "Message_Broker";
        string routingKey = "Broker_key";
        string queueName = "Message_queue";

        var rabbitMQConsumer = new RabbitMQConsumer(rabbitMQUri, exchangeName, routingKey, queueName);
        rabbitMQConsumer.StartConsuming();

        Console.WriteLine("Consumer is ready to recieve messages...\n");
        Console.WriteLine("Press Enter to stop consuming...");
        Console.ReadLine();

        rabbitMQConsumer.StopConsuming();


        // Insert a message into the database
        //var message = new Message
        //{
        //    Timestamp = DateTime.UtcNow,
        //    counter = 0
        //};

        //Database.InsertMessage(message);
    }
}