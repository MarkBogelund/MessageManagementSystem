using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using ConsumerApp.Models;

namespace ConsumerApp
{
    public class MessageBrokerConsumer
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

        public MessageBrokerConsumer(string uri, string exchangeName, string routingKey, string queueName)
        {
            // Create a connection to RabbitMQ
            var factory = new ConnectionFactory
            {
                Uri = new Uri(uri),
                ClientProvidedName = "Consumer"
            };

            // Setup connection to RabbitMQ
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
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

        public async Task<Message> StartConsumingAsync()
        {
            // Create a task completion source to await the next message
            var tcs = new TaskCompletionSource<Message>();

            // Set up a callback to be invoked when a message is received
            _consumer.Received += (sender, args) =>
            {
                try
                {
                    // Extract and decode the message from the event arguments
                    var body = args.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body);

                    var messageData = JsonConvert.DeserializeObject<Message>(message);

                    // If the task is not yet completed, set the result
                    if (!tcs.Task.IsCompleted)
                    {
                        if (messageData == null)
                        {
                            Console.WriteLine("Received message was null.");
                            return;
                        }

                        tcs.SetResult(messageData);

                        // Acknowledge the message after it was processed successfully
                        _channel.BasicAck(args.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    // Log the error or handle it as appropriate for your application
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };

            // Start consuming messages
            _channel.BasicConsume(_queueName, false, _consumer);

            // Await the next message
            return await tcs.Task;
        }

        public void SendMessageToQueue(Message message)
        {
            // Convert message to byte array and publish
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(_exchangeName, _routingKey, _properties, messageBodyBytes);
        }

        public void StopConsuming()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}