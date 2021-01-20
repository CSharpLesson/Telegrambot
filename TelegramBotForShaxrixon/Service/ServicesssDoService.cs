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
    public class ServicesssDoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Task<List<Servicess>> GetAll() => new DataContext().Services.ToListAsync();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static Servicess GetById(int Id) => new DataContext().Services.FirstOrDefault(f => f.Id == Id);


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
