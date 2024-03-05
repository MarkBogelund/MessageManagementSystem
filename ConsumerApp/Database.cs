using ConsumerApp.DataContext;
using ConsumerApp.Models;
using ConsumerApp.Interfaces;

namespace ConsumerApp
{
    public class Database(AppDBContext context) : IDatabase
    {
        public void InsertMessage(Message message)
        {          
            context.Messages.Add(message);
            context.SaveChanges();
        }
    }
}
