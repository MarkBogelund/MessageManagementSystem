using ConsumerApp.DataContext;
using ConsumerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsumerApp
{
    public class Database
    {
        private readonly AppDBContext _context;

        public Database(AppDBContext context)
        {
            _context = context;
        }

        public void InsertMessage(Message message)
        {
            // Insert the message using the injected context
            _context.Messages.Add(message);
            _context.SaveChanges();
        }
    }
}
