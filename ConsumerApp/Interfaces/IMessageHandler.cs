using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsumerApp.Models;

namespace ConsumerApp.Interfaces
{
    public interface IMessageHandler
    {
        void HandleMessage(Message message);
        int GetTimeDifference(Message message);
        void HandleData(Message message);
    }
}
