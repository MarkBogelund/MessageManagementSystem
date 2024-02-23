using System;
using System.Text;
using RabbitMQ.Client;

ConnectionFactory factory = new();

factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Publisher";

IConnection cnn = factory.CreateConnection();

IModel channel = cnn.CreateModel();

string exchangeName = "Message_Broker";
string routingKey = "Broker_key";
string queueName = "Message_queue";


channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, true, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);

var properties = channel.CreateBasicProperties();
properties.Persistent = true;

var counter = 0;

for (int i = 0; i < 10; i++)
{

    byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Hello message broker, Timestamp of message creation: {DateTime.Now} Counter: {counter} \n");
    channel.BasicPublish(exchangeName, routingKey, properties, messageBodyBytes);
    Console.WriteLine($" Sent '{messageBodyBytes}");
    Thread.Sleep(1000);
}

channel.Close();
cnn.Close();

