using NUnit.Framework;
using Moq;
using RabbitMQ.Client;
using System;
using ProducerApp;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework.Internal;

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
            // Mock RabbitMQ with its properties
            _mockConnection = new Mock<IConnection>();
            _mockChannel = new Mock<IModel>();

            _mockBasicProperties = new Mock<IBasicProperties>();
            _mockChannel.Setup(c => c.CreateBasicProperties()).Returns(_mockBasicProperties.Object);

            _mockChannel.Setup(c => c.BasicPublish("mock-exchange", "mock-routing-key", false, _mockBasicProperties.Object, It.IsAny<ReadOnlyMemory<byte>>()));

            // Mock the CreateConnection and CreateModel
            var mockConnectionFactory = new Mock<IConnectionFactory>();
            mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(_mockConnection.Object);
            _mockConnection.Setup(c => c.CreateModel()).Returns(_mockChannel.Object);

            _messageBrokerProducer = new MessageBrokerProducer(mockConnectionFactory.Object, "mock-exchange", "mock-routing-key", "mock-queue");


        }

        [Test]
        public void PublishMessage_MessageIsPublished()
        {
            // Arrange
            Message message = new Message
            {
                Id = 1,
                Counter = 1,
                Time = 1
            };

            // Act
            _messageBrokerProducer.PublishMessage(message);

            // Assert
            _mockChannel.Verify(c => c.BasicPublish("mock-exchange", "mock-routing-key", false, _mockBasicProperties.Object, It.IsAny<ReadOnlyMemory<byte>>()), Times.Once);
        }

        [Test]
        public void CloseConnection_ConnectionIsClosed()
        {
            // Act
            _messageBrokerProducer.CloseConnection();

            // Assert
            _mockChannel.Verify(c => c.Close(), Times.Once);
            _mockConnection.Verify(c => c.Close(), Times.Once);
        }

        [Test]
        public void CreateMessage_ReturnsValidMessage()
        {
            // Arrange
            int testId = 42;
            int testTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            Message expectedMessage = new Message
            {
                Id = testId,
                Counter = 0,
                Time = testTime
            };

            // Act
            Message result = _messageBrokerProducer.CreateMessage(testId);

            // Assert
            Assert.That(result.Counter, Is.EqualTo(expectedMessage.Counter));
            Assert.That(testId, Is.EqualTo(result.Id));
            Assert.That(testTime, Is.EqualTo(result.Time));
        }

    }

}
