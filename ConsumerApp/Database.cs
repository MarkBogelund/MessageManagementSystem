using ConsumerApp.DataContext;
using ConsumerApp.Models;
using System;

namespace ConsumerApp
{
    public class Database
    {
        public void InsertMessage(Message message)
        {
            using (var context = new AppDBContext())
            {
                context.Messages.Add(message);
                context.SaveChanges();
            }
        }
    }
}
