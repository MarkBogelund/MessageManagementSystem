using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConsumerApp.Models;

namespace ConsumerApp.DataContext
{
    internal class AppDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Host=localhost;Database=messagesDB;Username=postgres;Password=***********;Persist Security Info=True");
        }

        public DbSet<Message> Messages { get; set; }
    }
}
