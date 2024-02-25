using Microsoft.EntityFrameworkCore;
using ConsumerApp.Models;

namespace ConsumerApp.DataContext
{
    public class AppDBContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=postgres;Database=messagesDB;Username=postgres;Password=postgres");

            // Enable automatic migration execution
            optionsBuilder.EnableServiceProviderCaching(false);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your model here if needed
            modelBuilder.Entity<Message>().ToTable("Messages");
            modelBuilder.Entity<Message>().HasKey(m => m.Id);
            modelBuilder.Entity<Message>().Property(m => m.Counter).IsRequired();
            modelBuilder.Entity<Message>().Property(m => m.Time).IsRequired();
        }
    }
}
