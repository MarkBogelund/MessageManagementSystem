using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

ConnectionFactory factory = new();

factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Consumer";

IConnection cnn = factory.CreateConnection();

IModel channel = cnn.CreateModel();

string exchangeName = "Message_Broker";
string routingKey = "Broker_key";
string queueName = "Message_queue";


channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, true, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);
channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
    var body = args.Body.ToArray();

    string message = Encoding.UTF8.GetString(body);

    Console.Write($"Message Recieved: {message}");

    channel.BasicAck(args.DeliveryTag, false);
};

string consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
cnn.Close();
