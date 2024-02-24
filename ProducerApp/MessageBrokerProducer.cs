using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace ProducerApp
{
    public class MessageBrokerProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;
        private readonly string _routingKey;
        private readonly string _queueName;

        private IBasicProperties _properties;

        public MessageBrokerProducer(string uri, string exchangeName, string routingKey, string queueName)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(uri),
                ClientProvidedName = "Publisher"
            };

            // Create connection and channel
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchangeName = exchangeName;
            _routingKey = routingKey;
            _queueName = queueName;
            
            // Create properties and enable persistence
            _properties = _channel.CreateBasicProperties();
            _properties.Persistent = true;

            Initialize();
        }

        private void Initialize()
        {
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(_queueName, true, false, false, null);
            _channel.QueueBind(_queueName, _exchangeName, _routingKey, null);
        }

        public void PublishMessage(Message message)
        {
            // Convert message to byte array and publish
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(_exchangeName, _routingKey, _properties, messageBodyBytes);
        }

        public void CloseConnection()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}



