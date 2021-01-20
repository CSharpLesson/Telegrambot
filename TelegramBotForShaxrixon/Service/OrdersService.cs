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
        public  static Task<List<Orders>> GetAll() => new DataContext().Orders.ToListAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public  static Task<Orders> GetByChatId(long chatId) => new DataContext().Orders.FirstOrDefaultAsync(f => f.ChatId == chatId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public  static Task<Orders> GetByPositionChatId(long chatId, int position)
        {
            var date = DateTime.Now;
            return new DataContext().Orders.FirstOrDefaultAsync(f => f.ChatId == chatId && f.Position == position && f.DateOrder.Value.Day == date.Day && f.DateOrder.Value.Month == date.Month && f.DateOrder.Value.Year == date.Year);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public static Task<List<Orders>> GetAllByPositionChatId(long chatId, int position)
        {
            var date = DateTime.Now;
            return new DataContext().Orders.Where(f => f.ChatId == chatId && f.Position == position && f.DateOrder.Value.Day == date.Day && f.DateOrder.Value.Month == date.Month && f.DateOrder.Value.Year == date.Year).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public static Task<Orders> GetByPositionChatIdService(long chatId, int position, int service)
        {
            var date = DateTime.Now;
            return new DataContext().Orders.FirstOrDefaultAsync(f => f.ChatId == chatId && f.Position == position && f.ServiceId == service && f.DateOrder.Value.Day == date.Day && f.DateOrder.Value.Month == date.Month && f.DateOrder.Value.Year == date.Year);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public static Task<List<Orders>> GetByPositionChatIdDate(long chatId, int position)
        {
            var date = DateTime.Now;
            return new DataContext().Orders.Where(f => f.ChatId == chatId && f.Position == position && f.DateOrder.Value.Day == date.Day && f.DateOrder.Value.Month == date.Month)
                                           .Include(c => c.ServiceModel).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public static void AddOrUpdate(Orders model)
        {
            var context = new DataContext();
            context.Orders.Update(model);
            context.SaveChanges();
        }
    }
}
