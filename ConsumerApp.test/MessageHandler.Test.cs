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
        private IMessageBrokerConsumer messageBrokerMock;
        private IDatabase databaseMock;
        private MessageHandler messageHandler;
        private IMessageHandler messageHandlerMock;
        [SetUp]
        public void Setup()
        {
            messageBrokerMock = Mock.Of<IMessageBrokerConsumer>();
            databaseMock = Mock.Of<IDatabase>();
            messageHandler = new MessageHandler(messageBrokerMock, databaseMock);
            messageHandlerMock = Mock.Of<IMessageHandler>();
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
        public void MessageHandler_handleData_CallInsertMessage()
        {
            //arrange

            //act

            //assert

        }

        [Test]
        public void MessageHandler_HandleMessage_DiscardMessage()
        {
            // Arrange
            var messageHandlerMock = new Mock<MessageHandler>();
            var testMessage = new Message
            {
                Id = 0,
                Counter = 1,
                Time = 1000,
            };

            // Act
            messageHandlerMock.Object.HandleMessage(testMessage);

            // Assert
            messageHandlerMock.Verify(x => x.HandleData(It.IsAny<Message>()), Times.Never);

        }

        [Test]
        public void MessageHandler_HandleMessage_CallHandleData()
        {
            //arrange
            var timeMessage = new Message();
            timeMessage.Time = 1708887143; // time at 19:52 25/02/2024

            //act

            //assert

        }
    }
}