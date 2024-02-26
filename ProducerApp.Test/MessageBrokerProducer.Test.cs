using NUnit.Framework;
using Moq;
using RabbitMQ.Client;
using System;

namespace ProducerApp.Tests
{
    [TestFixture]
    public class MessageBrokerProducerTests
    {
        private Mock<IConnection> _mockConnection;
        private Mock<IModel> _mockChannel;
        private Mock<IBasicProperties> _mockBasicProperties;
        private MessageBrokerProducer _messageBrokerProducer;

        [SetUp]
        public void Setup()
        {
            // Mock the RabbitMQ connection and channel
            _mockConnection = new Mock<IConnection>();
            _mockChannel = new Mock<IModel>();

            // Mock the BasicProperties
            _mockBasicProperties = new Mock<IBasicProperties>();
            _mockChannel.Setup(c => c.CreateBasicProperties()).Returns(_mockBasicProperties.Object);

            // Set up the mock to expect a call to BasicPublish with specific arguments
            _mockChannel.Setup(c => c.BasicPublish("mock-exchange", "mock-routing-key", false, _mockBasicProperties.Object, It.IsAny<ReadOnlyMemory<byte>>()));

            // Mock the CreateConnection and CreateModel methods to return the mock connection and channel
            var mockConnectionFactory = new Mock<IConnectionFactory>();
            mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(_mockConnection.Object);
            _mockConnection.Setup(c => c.CreateModel()).Returns(_mockChannel.Object);

            // Create an instance of MessageBrokerProducer with the mock connection factory
            _messageBrokerProducer = new MessageBrokerProducer(mockConnectionFactory.Object, "mock-exchange", "mock-routing-key", "mock-queue");
        }

        [Test]
        public void PublishMessage_Success()
        {
            Message message = new Message
            {
                Id = 1,
                Counter = 1,
                Time = 1
            };

            // Act
            _messageBrokerProducer.PublishMessage(message);

            // Assert
            // Verify that BasicPublish was called with the expected arguments
            _mockChannel.Verify(c => c.BasicPublish("mock-exchange", "mock-routing-key", false, _mockBasicProperties.Object, It.IsAny<ReadOnlyMemory<byte>>()), Times.Once);
        }

        [Test]
        public void CloseConnection_Success()
        {
            // Act
            _messageBrokerProducer.CloseConnection();

            // Assert
            _mockChannel.Verify(c => c.Close(), Times.Once);
            _mockConnection.Verify(c => c.Close(), Times.Once);
        }
    }
}
