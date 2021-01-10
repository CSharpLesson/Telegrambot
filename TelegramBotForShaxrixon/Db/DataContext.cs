using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TelegramBotForShaxrixon.Model;

namespace TelegramBotForShaxrixon.Db
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=suxoyMoyka;Username=postgres;Password=123");    
        }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<Servicess> Services { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<Client> Clients { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<Company> Companies { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<Orders> Orders { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<Language> Languages { get; set; }
    }
}
