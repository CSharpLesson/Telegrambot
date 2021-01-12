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
        public static List<Client> GetAll()
        {
            try
            {
                return new DataContext().Clients.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public  async static Task<Client> GetByChatId(long chatId)
        {
            try
            {               
                 return await new DataContext().Clients.FirstOrDefaultAsync(f => f.ChatId == chatId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Client GetById(int Id)
        {
            try
            {
                return new DataContext().Clients.FirstOrDefault(f => f.Id == Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public async static Task AddOrUpdate(Client model) 
        {
            try
            {
                await Task.Run(() => 
                {
                    DataContext context = new DataContext();
                    context.Clients.Update(model);
                    context.SaveChanges();
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
