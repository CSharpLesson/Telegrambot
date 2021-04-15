using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotForShaxrixon.Db;
using TelegramBotForShaxrixon.Model;

namespace TelegramBotForShaxrixon.Service
{

    public class ClientService
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Task<List<Client>> GetAll() => new DataContext().Clients.ToListAsync();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public static Task<Client> GetByChatId(long chatId) => new DataContext().Clients.FirstOrDefaultAsync(f => f.ChatId == chatId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static Task<Client> GetById(int Id) => new DataContext().Clients.FirstOrDefaultAsync(f => f.Id == Id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public static void AddOrUpdate(Client model)
        {
            try
            {
                DataContext context = new DataContext();
                context.Clients.Update(model);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task<int> GetCount() => new DataContext().Clients.Where(f => f.DateCreate.Value.Date == DateTime.Now.Date).Count();
    }
}
