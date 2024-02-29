using ConsumerApp.Models;
using ConsumerApp;
using Moq;
using Moq.Protected;
using NUnit.Framework.Interfaces;
using ConsumerApp.Interfaces;
using Microsoft.Identity.Client;

namespace ConsumerApp.test
{
    public class MessageHandlerTests
    {
        private Mock<IMessageBrokerConsumer> messageBrokerMock;
        private Mock<IDatabase> databaseMock;
        private MessageHandler messageHandler;

        [SetUp]
        public void Setup()
        {
            messageBrokerMock = new Mock<IMessageBrokerConsumer>();
            databaseMock = new Mock<IDatabase>();
            messageHandler = new MessageHandler(messageBrokerMock.Object, databaseMock.Object);
        }

        //ClassName_MethodName_ExpectedResult
        [Test]
        public void MessageHandler_GetTimeDifference_ReturnsTimeDifference()
        {
            // Arrange
            var testMessage = new Message { Id = 1, Counter = 2, Time = 1708887143 }; // time at 19:52 25/02/2024

            // Act
            int result = messageHandler.GetTimeDifference(testMessage);

            // Assert
            int expectedTimeDifference =
                (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - testMessage.Time;

            Assert.That(expectedTimeDifference, Is.EqualTo(result));
        }

        [Test]
        public void MessageHandler_HandleMessage_DiscardMessage()
        {
            // Arrange
            var messageHandlerMock = new Mock<IMessageHandler>();
            var testMessage = new Message
            {
                Id = 0,
                Counter = 1,
                Time = 1000,
            };

            // Act
            messageHandlerMock.Object.HandleMessage(testMessage);

            // Assert that it has called HandleData once
            messageHandlerMock.Verify(x => x.HandleData(testMessage), Times.Never);
        }

        [Test]
        public void HandleMessage_EvenSeconds_InsertsIntoDatabase()
        {
            // Arrange
            var message = new Message { 
                Id = 1,
                Counter = 2,
                Time = 1708887144
            }; // Assuming even seconds

            // Act
            messageHandler.HandleMessage(message);

            // Assert
            databaseMock.Verify(d => d.InsertMessage(It.IsAny<Message>()), Times.Once);
            messageBrokerMock.Verify(m => m.SendMessageToQueue(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public void HandleMessage_OddSeconds_SendToMessageQueue()
        {
            // Arrange
            var message = new Message
            {
                Id = 1,
                Counter = 2,
                Time = 1708887145
            }; // Assuming odd seconds

            // Act
            messageHandler.HandleMessage(message);

            // Assert
            databaseMock.Verify(d => d.InsertMessage(It.IsAny<Message>()), Times.Never);
            messageBrokerMock.Verify(m => m.SendMessageToQueue(It.IsAny<Message>()), Times.Once);
        }

        [Test]
        public void MessageHandler_handleData_CallInsertMessage()
        {
            var testMessage = new Message { Id = 1, Counter = 2, Time = 1708887144 }; // seconds are even

            // Act
            messageHandler.HandleMessage(testMessage);

            // Assert
            databaseMock.Verify(x => x.InsertMessage(testMessage), Times.Once);
        }
        [Test]
        public void MessageHandler_handleData_CallSendBackToQueue()
        {
            var testMessage = new Message { Id = 1, Counter = 2, Time = 1708887145 }; // seconds are odd

            // Act
            messageHandler.HandleMessage(testMessage);

            // Assert
            messageBrokerMock.Verify(x => x.SendMessageToQueue(testMessage), Times.Once);
        }

    }
}