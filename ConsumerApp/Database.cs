using ConsumerApp.DataContext;
using ConsumerApp.Models;
using System;

namespace ConsumerApp
{
    public static class Database
    {
        public static void InsertMessage(MessageData message)
        {
            using (var context = new AppDBContext())
            {
                context.Messages.Add(message);
                context.SaveChanges();
            }
        }
    }
}
