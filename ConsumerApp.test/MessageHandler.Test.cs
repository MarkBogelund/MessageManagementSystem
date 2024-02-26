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
        public void MessageHandler_HandleMessage_CallInsertMessageThroughHandleData()
        {
            // Arrange
            var testTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // make sure seconds are even
            if (testTime % 2 == 0)
            { }
            else
            {
                testTime--;
            }
            var testMessage = new Message { Id = 1, Counter = 2, Time = testTime }; 

            //Act
            messageHandler.HandleMessage(testMessage);
            
            // Assert
            databaseMock.Verify(x => x.InsertMessage(testMessage), Times.Once);
        }

        [Test]
        public void MessageHandler_HandleMessage_CallSendMessageBackToQueueThroughHandleData()
        {
            // Arrange
            var testTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // make sure seconds are even
            if (testTime % 2 == 0)
            {
                testTime--;

            }
            else
            { }
            var testMessage = new Message { Id = 1, Counter = 2, Time = testTime };

            //Act
            messageHandler.HandleMessage(testMessage);

            // Assert
            messageBrokerMock.Verify(x => x.SendMessageToQueue(testMessage), Times.Once);
        }

        [Test]
        public void MessageHandler_handleData_CallInsertMessage()
        {
            var testMessage = new Message { Id = 1, Counter = 2, Time = 1708887144 }; // seconds are even

            // Act
            messageHandler.HandleData(testMessage);

            // Assert
            databaseMock.Verify(x => x.InsertMessage(testMessage), Times.Once);
        }
        [Test]
        public void MessageHandler_handleData_CallSendBackToQueue()
        {
            var testMessage = new Message { Id = 1, Counter = 2, Time = 1708887145 }; // seconds are odd

            // Act
            messageHandler.HandleData(testMessage);

            // Assert
            messageBrokerMock.Verify(x => x.SendMessageToQueue(testMessage), Times.Once);
        }

    }
}