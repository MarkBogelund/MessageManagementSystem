﻿using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using ConsumerApp.Models;
using ConsumerApp.Interfaces;

namespace ConsumerApp
{
    public class MessageBrokerConsumer : IMessageBrokerConsumer
    {
        // Declare RabbitMQ connection and channel
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;
        private readonly string _routingKey;
        private readonly string _queueName;
        private readonly EventingBasicConsumer _consumer;

        // Properties for sending messages to the queue
        private IBasicProperties _properties;

        public MessageBrokerConsumer(IConnectionFactory factory, string exchangeName, string routingKey, string queueName)
        {
            // Setup connection to RabbitMQ
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Setup connection to RabbitMQ
            _exchangeName = exchangeName;
            _routingKey = routingKey;
            _queueName = queueName;
            _consumer = new EventingBasicConsumer(_channel);

            // Set up properties for sending messages to the queue
            _properties = _channel.CreateBasicProperties();
            _properties.Persistent = true;

            Initialize();
        }

        private void Initialize()
        {
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(_queueName, true, false, false, null);
            _channel.QueueBind(_queueName, _exchangeName, _routingKey, null);
            _channel.BasicQos(0, 1, false);
        }

        public Task<Message> RecieveMessageAsync() // This is interacting with a boundary, so it's not a pure unit test
        {
            // Create task completion source to contain the message data
            var tcs = new TaskCompletionSource<Message>();

            // Set up a callback to be invoked when a message is received
            _consumer.Received += (sender, args) =>
            {
                if (sender == null) throw new Exception("Received message was null.");
                
                // Extract and decode the message from the event arguments
                Message message = ExtractMessageData(sender, args, _channel);

                if (tcs.Task.IsCompleted) return;

                // Acknowledge the message after it was processed successfully
                _channel.BasicAck(args.DeliveryTag, false);

                // Set the result of the task to the message data
                tcs.SetResult(message);
            };

            // Start consuming messages
            _channel.BasicConsume(_queueName, false, _consumer);

            // Return the task to await the next message
            return tcs.Task;
        }

        public Message ExtractMessageData(object sender, BasicDeliverEventArgs args, IModel channel)
        {
            // Extract and decode the message from the event arguments
            var body = args.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);

            var messageData = JsonConvert.DeserializeObject<Message>(message);

            if (messageData == null)
            {
                throw new Exception("Received message was null.");
            }

            return messageData;
        }

        public void SendMessageToQueue(Message message)
        {
            // Convert message to byte array and publish
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(_exchangeName, _routingKey, _properties, new ReadOnlyMemory<byte>(messageBodyBytes));
        }

        public void StopConsuming()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}