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
    public class OrdersService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async static Task<List<Orders>> GetAll()
        {
            try
            {
                return await new  DataContext().Orders.ToListAsync();
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
        public async static Task<Orders> GetByChatId(long chatId)
        {
            try
            {
                return await new DataContext().Orders.FirstOrDefaultAsync(f => f.ChatId == chatId);
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
        public async static  Task<Orders> GetByPositionChatId(long chatId, int position)
        {
            try
            {
                return await new DataContext().Orders.FirstOrDefaultAsync(f => f.ChatId == chatId && f.Position == position);
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
        public  async static Task<List<Orders>> GetAllByPositionChatId(long chatId, int position)
        {
            try
            {
                return  await new DataContext().Orders.Where(f => f.ChatId == chatId && f.Position == position).ToListAsync();
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
        public async static Task<Orders> GetByPositionChatIdService(long chatId, int position, int service)
        {
            try
            {
                var date = DateTime.Now;                
                  return  await new DataContext().Orders.FirstOrDefaultAsync(f => f.ChatId == chatId && f.Position == position && f.ServiceId == service && f.DateOrder.Value.Day == date.Day && f.DateOrder.Value.Month == date.Month);
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
        public async static Task<List<Orders>> GetByPositionChatIdDate(long chatId, int position)
        {
            try
            {
                var date = DateTime.Now;
                return await new DataContext().Orders.Where(f => f.ChatId == chatId && f.Position == position &&f.DateOrder.Value.Day==date.Day && f.DateOrder.Value.Month == date.Month)
                                               .Include(c=>c.ServiceModel).ToListAsync();
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
        public async static Task AddOrUpdate(Orders model)
        {
            try
            {
                var context = new DataContext();
                context.Orders.Update(model);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
