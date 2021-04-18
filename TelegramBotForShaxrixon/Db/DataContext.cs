using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBotForShaxrixon.Model;

namespace TelegramBotForShaxrixon.Db
{
    /// <summary>
    /// 
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        public DataContext()
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Program.DataCon);
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

        /// <summary>
        /// 
        /// </summary>
        public DbSet<TelegramBotToken> TelegramBotToken { get; set; }
    }
}
