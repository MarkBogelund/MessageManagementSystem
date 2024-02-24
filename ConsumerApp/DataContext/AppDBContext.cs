using Microsoft.EntityFrameworkCore;
using ConsumerApp.Models;

namespace ConsumerApp.DataContext
{
    internal class AppDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=messagesDB;Username=postgres;Password=postgres");
        }

        public DbSet<MessageEntry> Messages { get; set; }
    }
}