using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

public class RabbitMQPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchangeName;
    private readonly string _routingKey;
    private readonly string _queueName;
    private readonly int _messageCount;

    public RabbitMQPublisher(string uri, string exchangeName, string routingKey, string queueName, int messageCount)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(uri),
            ClientProvidedName = "Publisher"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _exchangeName = exchangeName;
        _routingKey = routingKey;
        _queueName = queueName;
        _messageCount = messageCount;

        Initialize();
    }

    private void Initialize()
    {
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(_queueName, true, false, false, null);
        _channel.QueueBind(_queueName, _exchangeName, _routingKey, null);
    }

    public void PublishMessages()
    {
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        var counter = 0;

        for (int i = 0; i < _messageCount; i++)
        {
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Hello message broker, Timestamp of message creation: {DateTime.Now} Counter: {counter} \n");
            _channel.BasicPublish(_exchangeName, _routingKey, properties, messageBodyBytes);
            Console.WriteLine($"Sent '{messageBodyBytes}'");
            counter++;
            Thread.Sleep(1000);
        }
    }

    public void CloseConnection()
    {
        _channel.Close();
        _connection.Close();
    }
}



