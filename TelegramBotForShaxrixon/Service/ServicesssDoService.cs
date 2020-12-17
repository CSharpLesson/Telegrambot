using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelegramBotForShaxrixon.Db;
using TelegramBotForShaxrixon.Model;

namespace TelegramBotForShaxrixon.Service
{
    public class ServicesssDoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<Servicess> GetAll()
        {
            try
            {
                return new DataContext().Services.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static Servicess GetById(int Id)
        {
            try
            {
                return new DataContext().Services.FirstOrDefault(f => f.Id == Id);
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
        public static void AddOrUpdate(Servicess model)
        {
            try
            {
                var context = new DataContext();
                context.Services.Update(model);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
