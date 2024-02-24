using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using ConsumerApp.Models;

namespace ConsumerApp
{
    public class RabbitMQConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;
        private readonly string _routingKey;
        private readonly string _queueName;

        private readonly EventingBasicConsumer _consumer;

        public RabbitMQConsumer(string uri, string exchangeName, string routingKey, string queueName)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(uri),
                ClientProvidedName = "Consumer"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchangeName = exchangeName;
            _routingKey = routingKey;
            _queueName = queueName;

            Initialize();

            _consumer = new EventingBasicConsumer(_channel);
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
            var tcs = new TaskCompletionSource<Message>();

            _consumer.Received += (sender, args) =>
            {
                try
                {
                    var body = args.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body);

                    var messageData = JsonConvert.DeserializeObject<Message>(message);

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

            _channel.BasicConsume(_queueName, false, _consumer);

            return await tcs.Task;
        }

        public void StopConsuming()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}