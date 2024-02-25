using ConsumerApp.DataContext;
using ConsumerApp.Models;
using System;
using ConsumerApp.Interfaces;

namespace ConsumerApp
{
    public class Database : IDatabase
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
