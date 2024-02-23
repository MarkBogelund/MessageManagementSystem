using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMQConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchangeName;
    private readonly string _routingKey;
    private readonly string _queueName;

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
    }

    private void Initialize()
    {
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(_queueName, true, false, false, null);
        _channel.QueueBind(_queueName, _exchangeName, _routingKey, null);
        _channel.BasicQos(0, 1, false);
    }

    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (sender, args) =>
        {
            var body = args.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Message Received: {message}");
            _channel.BasicAck(args.DeliveryTag, false);
        };

        string consumerTag = _channel.BasicConsume(_queueName, false, consumer);
    }

    public void StopConsuming()
    {
        _channel.Close();
        _connection.Close();
    }
}
