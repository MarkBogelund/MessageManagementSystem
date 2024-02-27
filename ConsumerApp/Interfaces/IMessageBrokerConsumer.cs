using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsumerApp.Models;

namespace ConsumerApp.Interfaces
{
    public interface IMessageBrokerConsumer
    {
        Task<Message> RecieveMessageAsync();
        void SendMessageToQueue(Message message);
        void StopConsuming();
    }
}
