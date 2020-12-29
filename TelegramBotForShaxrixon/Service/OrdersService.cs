using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public static List<Orders> GetAll()
        {
            try
            {
                return new DataContext().Orders.ToList();
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
        public static Orders GetByChatId(long chatId)
        {
            try
            {
                return new DataContext().Orders.FirstOrDefault(f => f.ChatId == chatId);
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
        public static Orders GetByPositionChatId(long chatId, int position)
        {
            try
            {
                return new DataContext().Orders.FirstOrDefault(f => f.ChatId == chatId && f.Position == position);
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
        public static List<Orders> GetAllByPositionChatId(long chatId, int position)
        {
            try
            {
                return new DataContext().Orders.Where(f => f.ChatId == chatId && f.Position == position).ToList();
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
        public static Orders GetByPositionChatIdService(long chatId, int position, int service)
        {
            try
            {
                var date = DateTime.Now;
                return new DataContext().Orders.FirstOrDefault(f => f.ChatId == chatId && f.Position == position && f.ServiceId == service && f.DateOrder.Value.Day == date.Day && f.DateOrder.Value.Month == date.Month);
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
        public static List<Orders> GetByPositionChatIdDate(long chatId, int position)
        {
            try
            {
                var date = DateTime.Now;
                return new DataContext().Orders.Where(f => f.ChatId == chatId && f.Position == position &&f.DateOrder.Value.Day==date.Day && f.DateOrder.Value.Month == date.Month)
                                               .Include(c=>c.ServiceModel).ToList();
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
        public static void AddOrUpdate(Orders model)
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
