using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelegramBotForShaxrixon.Db;
using TelegramBotForShaxrixon.Model;

namespace TelegramBotForShaxrixon.Service
{
    public class CompanyService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<Company> GetAll()
        {
            try
            {
                return new DataContext().Companies.ToList();
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
        public static Company GetByChatId(long chatId)
        {
            try
            {
                return new DataContext().Companies.FirstOrDefault(f => f.ChatId == chatId);
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
        public static void AddOrUpdate(Company model)
        {
            try
            {
                var context = new DataContext();
                context.Companies.Update(model);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
